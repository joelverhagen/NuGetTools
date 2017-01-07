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
        private readonly IFrameworkList _frameworkList;

        private readonly Lazy<Task<Dictionary<string, NuGetVersion>>> _versions;
        private readonly Lazy<Task<List<string>>> _versionStrings;

        private readonly ConcurrentDictionary<NuGetVersion, Logic> _logic
            = new ConcurrentDictionary<NuGetVersion, Logic>();

        private readonly ConcurrentDictionary<NuGetVersion, IToolsService> _toolServices
            = new ConcurrentDictionary<NuGetVersion, IToolsService>();

        private readonly ConcurrentDictionary<NuGetVersion, IFrameworkPrecedenceService> _frameworkPrecendenceServices
            = new ConcurrentDictionary<NuGetVersion, IFrameworkPrecedenceService>();
        
        public ToolsFactory(IPackageLoader packageLoader, IAlignedVersionsDownloader downloader, IFrameworkList frameworkList)
        {
            _packageLoader = packageLoader;
            _downloader = downloader;
            _frameworkList = frameworkList;

            _versions = new Lazy<Task<Dictionary<string, NuGetVersion>>>(async () =>
            {
                var versions = await _downloader.GetDownloadedVersionsAsync(
                    PackageIds,
                    CancellationToken.None);

                return versions
                    .ToDictionary(x => x.ToNormalizedString(), StringComparer.OrdinalIgnoreCase);
            });

            _versionStrings = new Lazy<Task<List<string>>>(async () =>
            {
                var versions = await _versions.Value;

                return versions
                    .OrderByDescending(x => x.Value)
                    .Select(x => x.Key)
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
            var matchingVersion = await GetMatchingVersionAsync(version);

            if (matchingVersion == null)
            {
                return null;
            }

            return _toolServices.GetOrAdd(
                matchingVersion,
                key =>
                {
                    var logic = GetLogic(key);

                    return new ToolsService<Framework, Version, VersionRange>(
                        version,
                        logic.Framework,
                        logic.Version,
                        logic.VersionRange);
                });
        }

        public async Task<IFrameworkPrecedenceService> GetFrameworkPrecedenceServiceAsync(string version, CancellationToken token)
        {
            var matchingVersion = await GetMatchingVersionAsync(version);

            if (matchingVersion == null)
            {
                return null;
            }
            
            return _frameworkPrecendenceServices.GetOrAdd(
                matchingVersion,
                key =>
                {
                    var logic = GetLogic(key);

                    return new FrameworkPrecedenceService<Framework>(
                        version,
                        _frameworkList,
                        logic.Framework);
                });
        }

        private Logic GetLogic(NuGetVersion version)
        {
            return _logic.GetOrAdd(version, GetLogicWithoutCaching);
        }

        private async Task<NuGetVersion> GetMatchingVersionAsync(string version)
        {
            var versions = await _versions.Value;
            NuGetVersion matchedVersion;
            if (!versions.TryGetValue(version, out matchedVersion))
            {
                return null;
            }

            return matchedVersion;
        }

        private Logic GetLogicWithoutCaching(NuGetVersion version)
        {
            // Load the needed assemblies.
            var versioningIdentity = new PackageIdentity(_versioningId, version);
            _packageLoader.LoadPackageAssemblies(
                version.ToNormalizedString(),
                _framework,
                versioningIdentity);

            var frameworksIdentity = new PackageIdentity(_frameworksId, version);
            var context = _packageLoader.LoadPackageAssemblies(
                version.ToNormalizedString(),
                _framework,
                frameworksIdentity);

            // Find the framework names of the needed assemblies.
            var versioningAssemblyName = context.LoadedAssemblies.GetByName(_versioningId);
            var frameworksAssemblyName = context.LoadedAssemblies.GetByName(_frameworksId);

            // Load the logic implementations.
            var frameworkLogic = context.Proxy.GetFrameworkLogic(frameworksAssemblyName);
            var versionLogic = context.Proxy.GetVersionLogic(versioningAssemblyName);
            var versionRangeLogic = context.Proxy.GetVersionRangeLogic(versioningAssemblyName);

            return new Logic
            {
                Framework = frameworkLogic,
                Version = versionLogic,
                VersionRange = versionRangeLogic
            };
        }

        private class Logic
        {
            public FrameworkLogic Framework { get; set; }
            public VersionLogic Version { get; set; }
            public VersionRangeLogic VersionRange { get; set; }
        }
    }
}
