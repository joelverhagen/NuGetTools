using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;
using Knapcode.NuGetTools.Logic.NuGet2x;
using Knapcode.NuGetTools.Logic.NuGet3x;
using Knapcode.NuGetTools.Logic.Wrappers;
using Microsoft.Extensions.Logging;
using Mono.Cecil;
using NuGet.Client;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Direct;

public class VersionedToolsFactory : IToolsFactory
{
    public static readonly IReadOnlyList<NuGetFramework> CompatibleFrameworks = new[]
    {
        NuGetFramework.Parse("net48"),
        NuGetFramework.Parse("net8.0"),
    };

    private static readonly Lazy<ModuleDefinition> NuGetLogic2xModule
        = new Lazy<ModuleDefinition>(() => ModuleDefinition.ReadModule(typeof(NuGetLogic2x).Assembly.Location));
    private static readonly Lazy<ModuleDefinition> NuGetLogic3xModule
        = new Lazy<ModuleDefinition>(() => ModuleDefinition.ReadModule(typeof(NuGetLogic3x).Assembly.Location));

    private static readonly ConcurrentDictionary<NuGetVersion, VersionContext> Contexts = new();

    private readonly IAlignedVersionsDownloader _downloader;
    private readonly IFrameworkList _frameworkList;
    private readonly NuGetSettings _settings;
    private readonly Lazy<Task<Dictionary<string, NuGetVersion>>> _stringToVersion;
    private readonly Lazy<Task<List<string>>> _versionStrings;
    private readonly Lazy<Task<string>> _latestVersion;
    private readonly Lazy<Task<IReadOnlySet<NuGetVersion>>> _versionSet;

    private readonly ConcurrentDictionary<NuGetVersion, Task<IToolsService>> _toolServices = new();
    private readonly ConcurrentDictionary<NuGetVersion, Task<IFrameworkPrecedenceService>> _frameworkPrecedenceServices = new();
    private readonly MicrosoftLogger _nuGetLog;

    public VersionedToolsFactory(
        IAlignedVersionsDownloader downloader,
        IFrameworkList frameworkList,
        NuGetSettings settings,
        ILogger<VersionedToolsFactory> log)
    {
        _downloader = downloader;
        _frameworkList = frameworkList;
        _settings = settings;
        _nuGetLog = new MicrosoftLogger(log);

        _versionSet = new Lazy<Task<IReadOnlySet<NuGetVersion>>>(async () =>
        {
            using (var sourceCacheContext = new SourceCacheContext())
            {
                var versions2x = await _downloader.GetDownloadedVersionsAsync(
                    Constants.PackageIds2x,
                    sourceCacheContext,
                    _nuGetLog,
                    CancellationToken.None);

                var versions3x = await _downloader.GetDownloadedVersionsAsync(
                    Constants.PackageIds3x,
                    sourceCacheContext,
                    _nuGetLog,
                    CancellationToken.None);

                return versions2x.Concat(versions3x).ToHashSet();
            }
        });

        _stringToVersion = new Lazy<Task<Dictionary<string, NuGetVersion>>>(async () =>
        {
            var releases = await _versionSet.Value;

            return releases
                .ToDictionary(
                    x => x.ToNormalizedString(),
                    x => x,
                    StringComparer.OrdinalIgnoreCase);
        });

        _versionStrings = new Lazy<Task<List<string>>>(async () =>
        {
            var versions = await _stringToVersion.Value;

            return versions
                .OrderByDescending(x => x.Value)
                .Select(x => x.Key)
                .ToList();
        });

        _latestVersion = new Lazy<Task<string>>(async () =>
        {
            var versions = await _stringToVersion.Value;

            return versions
                .OrderByDescending(x => x.Value)
                .First()
                .Key;
        });
    }

    public async Task<IEnumerable<string>> GetAvailableVersionsAsync(CancellationToken token)
    {
        return await _versionStrings.Value;
    }

    public async Task<IToolsService?> GetServiceAsync(string version, CancellationToken token)
    {
        var matchingVersion = await GetMatchingVersionAsync(version);

        if (matchingVersion == null)
        {
            return null;
        }

        try
        {
            return await _toolServices.GetOrAdd(
                matchingVersion,
                async key =>
                {
                    var logic = await InitializeAndGetLogicAsync(key);
                    return new ToolsService(version, logic);
                });
        }
        catch
        {
            _toolServices.TryRemove(matchingVersion, out var _);
            throw;
        }
    }

    public async Task<IFrameworkPrecedenceService?> GetFrameworkPrecedenceServiceAsync(string version, CancellationToken token)
    {
        var matchingVersion = await GetMatchingVersionAsync(version);

        if (matchingVersion == null)
        {
            return null;
        }

        try
        {
            return await _frameworkPrecedenceServices.GetOrAdd(
                matchingVersion,
                async key =>
                {
                    var logic = await InitializeAndGetLogicAsync(key);
                    return new FrameworkPrecedenceService(
                        version,
                        _frameworkList,
                        logic.Framework);
                });
        }
        catch
        {
            _frameworkPrecedenceServices.TryRemove(matchingVersion, out var _);
            throw;
        }
    }

    public Task<IFrameworkList> GetFrameworkListAsync(CancellationToken token)
    {
        return Task.FromResult(_frameworkList);
    }

    public Task<string> GetLatestVersionAsync(CancellationToken token)
    {
        return _latestVersion.Value;
    }

    private async Task<NuGetVersion?> GetMatchingVersionAsync(string version)
    {
        var versions = await _stringToVersion.Value;
        NuGetVersion? matchedVersion;
        if (!versions.TryGetValue(version, out matchedVersion))
        {
            return null;
        }

        return matchedVersion;
    }

    private async Task<INuGetLogic> InitializeAndGetLogicAsync(NuGetVersion version)
    {
        await _versionSet.Value;

        var context = await GetContextAsync(_settings.GlobalPackagesFolder, version);

        return context.Logic!;
    }

    public static async Task<IEnumerable<Assembly>> GetLoadedAssembliesAsync(string packagesFolder, NuGetVersion version)
    {
        var context = await GetContextAsync(packagesFolder, version);

        return context.AssemblyLoadContext!.Assemblies;
    }

    private static async Task<VersionContext> GetContextAsync(string packagesFolder, NuGetVersion version)
    {
        var context = Contexts.GetOrAdd(version, _ => new VersionContext());
        if (context.Logic is not null)
        {
            return context;
        }

        await context.Lock.WaitAsync();
        try
        {
            if (context.Logic is not null)
            {
                return context;
            }

            return InitializeContext(packagesFolder, version, context);
        }
        finally
        {
            context.Lock.Release();
        }
    }

    private static VersionContext InitializeContext(string packagesFolder, NuGetVersion version, VersionContext context)
    {
        var release = version.Major >= 3 ? NuGetRelease.Version3x : NuGetRelease.Version2x;

        var assemblyLoadContext = new AssemblyLoadContext(
            name: $"NuGet {version.ToNormalizedString()}",
            isCollectible: false);

        var logicAssembly = release switch
        {
            NuGetRelease.Version2x => GetV2Implementation(packagesFolder, version, assemblyLoadContext),
            NuGetRelease.Version3x => GetV3Implementation(packagesFolder, version, assemblyLoadContext),
            _ => throw new NotImplementedException(),
        };

        var logicType = logicAssembly
            .ExportedTypes
            .First(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(INuGetLogic)));

        context.Logic = (INuGetLogic)Activator.CreateInstance(logicType)!;
        context.AssemblyLoadContext = assemblyLoadContext;

        return context;
    }

    private static Assembly GetV2Implementation(string packagesFolder, NuGetVersion version, AssemblyLoadContext context)
    {
        var coreIdentity = new PackageIdentity(Constants.CoreId, version);
        var assemblies = LoadPackageAssemblies(packagesFolder, coreIdentity, context);

        var logicAssembly = RewriteProxyReferences(context, NuGetLogic2xModule, assemblies);
        return logicAssembly;
    }

    private static Assembly GetV3Implementation(string packagesFolder, NuGetVersion version, AssemblyLoadContext context)
    {
        var versioningIdentity = new PackageIdentity(Constants.VersioningId, version);
        var assemblies = LoadPackageAssemblies(packagesFolder, versioningIdentity, context);

        var frameworksIdentity = new PackageIdentity(Constants.FrameworksId, version);
        assemblies.AddRange(LoadPackageAssemblies(packagesFolder, frameworksIdentity, context));

        var logicAssembly = RewriteProxyReferences(context, NuGetLogic3xModule, assemblies);
        return logicAssembly;
    }

    private static Assembly RewriteProxyReferences(
        AssemblyLoadContext context,
        Lazy<ModuleDefinition> lazyBaseModule,
        List<Assembly> newReferences)
    {
        using var moduleStream = new MemoryStream();

        lock (lazyBaseModule)
        {
            var baseModule = lazyBaseModule.Value;

            foreach (var reference in newReferences)
            {
                var referenceName = reference.GetName();
                var matching = baseModule.AssemblyReferences.FirstOrDefault(x => x.Name == referenceName.Name);
                if (matching is not null)
                {
                    matching.Version = referenceName.Version;
                }
            }

            baseModule.Write(moduleStream);
        }

        moduleStream.Position = 0;

        return context.LoadFromStream(moduleStream);
    }

    private static List<Assembly> LoadPackageAssemblies(string packagesFolder, PackageIdentity packageIdentity, AssemblyLoadContext context)
    {
        var pathResolver = new VersionFolderPathResolver(packagesFolder);
        var hashPath = pathResolver.GetHashPath(packageIdentity.Id, packageIdentity.Version);

        if (!File.Exists(hashPath))
        {
            throw new InvalidOperationException($"The package '{packageIdentity}' could not found.");
        }

        var installPath = pathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);

        using (var packageReader = new PackageFolderReader(installPath))
        {
            foreach (var framework in CompatibleFrameworks)
            {
                if (TryLoadWithFramework(context, framework, installPath, packageReader, out var assemblies))
                {
                    return assemblies;
                }
            }

            var frameworks = string.Join(", ", CompatibleFrameworks.Select(x => x.GetShortFolderName()));
            throw new InvalidOperationException(
                $"The package {packageIdentity.Id} {packageIdentity.Version} is not compatible " +
                $"with any of the following frameworks: {frameworks}");
        }
    }

    private static bool TryLoadWithFramework(
        AssemblyLoadContext context,
        NuGetFramework framework,
        string installPath,
        PackageFolderReader packageReader,
        [NotNullWhen(true)] out List<Assembly>? assemblies)
    {
        var conventions = new ManagedCodeConventions(null);
        var criteria = conventions.Criteria.ForFramework(framework);

        const char AssetDirectorySeparator = '/';
        var files = packageReader
            .GetFiles()
            .Select(p => p.Replace(Path.DirectorySeparatorChar, AssetDirectorySeparator))
            .ToList();

        var contentItems = new ContentItemCollection();
        contentItems.Load(files);

        var runtimeGroup = contentItems.FindBestItemGroup(
            criteria,
            conventions.Patterns.RuntimeAssemblies);

        if (runtimeGroup is null)
        {
            assemblies = null;
            return false;
        }

        assemblies = new List<Assembly>();
        foreach (var asset in runtimeGroup.Items)
        {
            var absolutePath = Path.Combine(
                installPath,
                asset.Path.Replace(AssetDirectorySeparator, Path.DirectorySeparatorChar));
            assemblies.Add(context.LoadFromAssemblyPath(absolutePath));
        }

        return true;
    }

    private class VersionContext
    {
        public SemaphoreSlim Lock { get; } = new SemaphoreSlim(initialCount: 1);
        public AssemblyLoadContext? AssemblyLoadContext { get; set; }
        public INuGetLogic? Logic { get; set; }
    }
}
