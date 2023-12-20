using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;
using Knapcode.NuGetTools.Logic.NuGet2x;
using Knapcode.NuGetTools.Logic.NuGet3x;
using Knapcode.NuGetTools.Logic.Wrappers;
using Microsoft.Extensions.Logging;
using NuGet.Client;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class ToolsFactory : IToolsFactory
    {
        private static readonly NuGetFramework Net48 = NuGetFramework.Parse("net48");
        private static readonly NuGetFramework Net6 = NuGetFramework.Parse("net6.0");

        private static readonly Lazy<byte[]> NuGet2xAssembly = new Lazy<byte[]>(() => File.ReadAllBytes(typeof(NuGetLogic2x).Assembly.Location));
        private static readonly Lazy<byte[]> NuGet3xAssembly = new Lazy<byte[]>(() => File.ReadAllBytes(typeof(NuGetLogic3x).Assembly.Location));

        private readonly IAlignedVersionsDownloader _downloader;
        private readonly IFrameworkList _frameworkList;
        private readonly NuGetSettings _settings;
        private readonly Lazy<Task<Dictionary<string, NuGetVersion>>> _versions;
        private readonly Lazy<Task<List<string>>> _versionStrings;
        private readonly Lazy<Task<string>> _latestVersion;
        private readonly Lazy<Task<Dictionary<NuGetVersion, NuGetRelease>>> _releases;

        private readonly ConcurrentDictionary<string, Lazy<AssemblyLoadContext>> _lazyContexts
            = new ConcurrentDictionary<string, Lazy<AssemblyLoadContext>>();

        private readonly ConcurrentDictionary<NuGetVersion, Task<INuGetLogic>> _logic
            = new ConcurrentDictionary<NuGetVersion, Task<INuGetLogic>>();

        private readonly ConcurrentDictionary<NuGetVersion, Task<IToolsService>> _toolServices
            = new ConcurrentDictionary<NuGetVersion, Task<IToolsService>>();

        private readonly ConcurrentDictionary<NuGetVersion, Task<IFrameworkPrecedenceService>> _frameworkPrecendenceServices
            = new ConcurrentDictionary<NuGetVersion, Task<IFrameworkPrecedenceService>>();
        private readonly MicrosoftLogger _nuGetLog;

        public ToolsFactory(
            IAlignedVersionsDownloader downloader,
            IFrameworkList frameworkList,
            NuGetSettings settings,
            ILogger<ToolsFactory> log)
        {
            _downloader = downloader;
            _frameworkList = frameworkList;
            _settings = settings;
            _nuGetLog = new MicrosoftLogger(log);

            _releases = new Lazy<Task<Dictionary<NuGetVersion, NuGetRelease>>>(async () =>
            {
                using (var sourceCacheContext = new SourceCacheContext())
                {
                    var versions2x = await _downloader.GetDownloadedVersionsAsync(
                        Constants.PackageIds2x,
                        sourceCacheContext,
                        _nuGetLog,
                        CancellationToken.None);
                    var pairs2x = versions2x
                        .Select(x => new KeyValuePair<NuGetVersion, NuGetRelease>(x, NuGetRelease.Version2x));

                    var versions3x = await _downloader.GetDownloadedVersionsAsync(
                        Constants.PackageIds3x,
                        sourceCacheContext,
                        _nuGetLog,
                        CancellationToken.None);
                    var pairs3x = versions3x
                        .Select(x => new KeyValuePair<NuGetVersion, NuGetRelease>(x, NuGetRelease.Version3x));

                    return pairs2x
                        .Concat(pairs3x)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            });

            _versions = new Lazy<Task<Dictionary<string, NuGetVersion>>>(async () =>
            {
                var releases = await _releases.Value;

                return releases
                    .ToDictionary(
                        x => x.Key.ToNormalizedString(),
                        x => x.Key,
                        StringComparer.OrdinalIgnoreCase);
            });

            _versionStrings = new Lazy<Task<List<string>>>(async () =>
            {
                var versions = await _versions.Value;

                return versions
                    .OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
                    .ToList();
            });

            _latestVersion = new Lazy<Task<string>>(async () =>
            {
                var versions = await _versions.Value;

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
                        var logic = await GetLogicAsync(key);
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
                return await _frameworkPrecendenceServices.GetOrAdd(
                    matchingVersion,
                    async key =>
                    {
                        var logic = await GetLogicAsync(key);
                        return new FrameworkPrecedenceService(
                            version,
                            _frameworkList,
                            logic.Framework);
                    });
            }
            catch
            {
                _frameworkPrecendenceServices.TryRemove(matchingVersion, out var _);
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

        private async Task<INuGetLogic> GetLogicAsync(NuGetVersion version)
        {
            try
            {
                return await _logic.GetOrAdd(version, GetLogicWithoutCachingAsync);
            }
            catch
            {
                _logic.TryRemove(version, out var _);
                throw;
            }
        }

        private async Task<NuGetVersion?> GetMatchingVersionAsync(string version)
        {
            var versions = await _versions.Value;
            NuGetVersion? matchedVersion;
            if (!versions.TryGetValue(version, out matchedVersion))
            {
                return null;
            }

            return matchedVersion;
        }

        private async Task<INuGetLogic> GetLogicWithoutCachingAsync(NuGetVersion version)
        {
            var releases = await _releases.Value;
            NuGetRelease release;
            if (!releases.TryGetValue(version, out release))
            {
                throw new ArgumentException($"The provided version '{version}' is not supported");
            }

            var contextId = version.ToNormalizedString();
            var context = GetAssemblyLoadContext(contextId);

            var logicAssembly = release switch
            {
                NuGetRelease.Version2x => GetV2Implementation(context, version),
                NuGetRelease.Version3x => GetV3Implementation(context, version),
                _ => throw new NotImplementedException(),
            };

            var logicType = logicAssembly
                .GetTypes()
                .First(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(INuGetLogic)));

            return (INuGetLogic)Activator.CreateInstance(logicType)!;
        }

        private Assembly GetV2Implementation(AssemblyLoadContext context, NuGetVersion version)
        {
            var coreIdentity = new PackageIdentity(Constants.CoreId, version);
            LoadPackageAssemblies(context, coreIdentity);

            using var memoryStream = new MemoryStream(NuGet2xAssembly.Value);
            var logicAssembly = context.LoadFromStream(memoryStream);
            return logicAssembly;
        }

        private Assembly GetV3Implementation(AssemblyLoadContext context, NuGetVersion version)
        {
            var versioningIdentity = new PackageIdentity(Constants.VersioningId, version);
            LoadPackageAssemblies(context, versioningIdentity);

            var frameworksIdentity = new PackageIdentity(Constants.FrameworksId, version);
            LoadPackageAssemblies(context, frameworksIdentity);

            using var memoryStream = new MemoryStream(NuGet3xAssembly.Value);
            var logicAssembly = context.LoadFromStream(memoryStream);
            return logicAssembly;
        }

        private AssemblyLoadContext GetAssemblyLoadContext(string contextId)
        {
            var lazyContext = _lazyContexts.GetOrAdd(contextId, new Lazy<AssemblyLoadContext>(() => new AssemblyLoadContext(
                contextId + ' ' + Guid.NewGuid(),
                isCollectible: false)));
            return lazyContext.Value;
        }

        private void LoadPackageAssemblies(
            AssemblyLoadContext context,
            PackageIdentity packageIdentity)
        {
            var pathResolver = new VersionFolderPathResolver(_settings.GlobalPackagesFolder);
            var hashPath = pathResolver.GetHashPath(packageIdentity.Id, packageIdentity.Version);

            if (!File.Exists(hashPath))
            {
                throw new InvalidOperationException($"The package '{packageIdentity}' could not found.");
            }

            var installPath = pathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);

            using (var packageReader = new PackageFolderReader(installPath))
            {
                if (!LoadWithFramework(context, Net6, installPath, packageReader)
                    && !LoadWithFramework(context, Net48, installPath, packageReader))
                {
                    throw new InvalidOperationException($"The package '{packageIdentity}' is not compatible with net6.0 or net48.");
                }
            }
        }

        private static bool LoadWithFramework(
            AssemblyLoadContext context,
            NuGetFramework framework,
            string installPath,
            PackageFolderReader packageReader)
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
                return false;
            }

            foreach (var asset in runtimeGroup.Items)
            {
                var absolutePath = Path.Combine(
                    installPath,
                    asset.Path.Replace(AssetDirectorySeparator, Path.DirectorySeparatorChar));
                context.LoadFromAssemblyPath(absolutePath);
            }

            return true;
        }
    }
}
