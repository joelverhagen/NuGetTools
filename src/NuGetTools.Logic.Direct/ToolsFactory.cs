using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;
using Version = Knapcode.NuGetTools.Logic.Wrappers.Reflection.Version;
using VersionRange = Knapcode.NuGetTools.Logic.Wrappers.Reflection.VersionRange;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class ToolsFactory : IToolsFactory
    {
        private const string _versioningId = "NuGet.Versioning";
        private const string _frameworksId = "NuGet.Frameworks";
        private static NuGetFramework _framework = FrameworkConstants.CommonFrameworks.Net46;

        private readonly IAlignedVersionsDownloader _downloader;
        private readonly IPackageLoader _packageLoader;

        private readonly Lazy<Task<List<NuGetVersion>>> _versions;
        private readonly Lazy<Task<List<string>>> _versionStrings;
        private readonly ConcurrentDictionary<string, Task<IToolsService>> _services
            = new ConcurrentDictionary<string, Task<IToolsService>>();

        public ToolsFactory(IPackageLoader packageLoader, IAlignedVersionsDownloader downloader)
        {
            _packageLoader = packageLoader;
            _downloader = downloader;

            _versions = new Lazy<Task<List<NuGetVersion>>>(async () =>
            {
                var versions = await _downloader.GetDownloadedVersionsAsync(
                    PackageIds,
                    CancellationToken.None);

                return versions
                    .OrderByDescending(x => x)
                    .ToList();
            });

            _versionStrings = new Lazy<Task<List<string>>>(async () =>
            {
                var versions = await _versions.Value;

                return versions
                    .Select(x => x.ToString())
                    .ToList();
            });
        }

        public static IEnumerable<string> PackageIds { get; } = new[]
        {
            _versioningId,
            _frameworksId
        };

        public async Task<IEnumerable<string>> GetAvailableVersionsAsync(CancellationToken token)
        {
            return await _versionStrings.Value;
        }

        public async Task<IToolsService> GetServiceAsync(string version, CancellationToken token)
        {
            return await _services.GetOrAdd(version, GetServiceWithoutCachingAsync);
        }

        private async Task<IToolsService> GetServiceWithoutCachingAsync(string version)
        {
            // Find the matching version.
            var versions = await _versions.Value;

            var matchedVersion = versions.FirstOrDefault(x => x.ToString() == version);
            if (matchedVersion == null)
            {
                return null;
            }

            // Load the needed assemblies.
            var versioningIdentity = new PackageIdentity(_versioningId, matchedVersion);
            _packageLoader.LoadPackageAssemblies(
                version,
                _framework,
                versioningIdentity);

            var frameworksIdentity = new PackageIdentity(_frameworksId, matchedVersion);
            var context = _packageLoader.LoadPackageAssemblies(
                version,
                _framework,
                frameworksIdentity);

            // Find the framework names of the needed assemblies.
            var versioningAssemblyName = context.LoadedAssemblies.First(x => x.Name == _versioningId);
            var frameworksAssemblyName = context.LoadedAssemblies.First(x => x.Name == _frameworksId);

            // Load the logic implementations.
            var frameworkLogic = context.Proxy.GetFrameworkLogic(frameworksAssemblyName);
            var versionLogic = context.Proxy.GetVersionLogic(versioningAssemblyName);
            var versionRangeLogic = context.Proxy.GetVersionRangeLogic(versioningAssemblyName);

            // Initialize the tools service.
            var toolsService = new ToolsService<Framework, Version, VersionRange>(
                version,
                frameworkLogic,
                versionLogic,
                versionRangeLogic);

            return toolsService;
        }
    }
}
