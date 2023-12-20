using NuGet.Common;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NuGetVersionRange = NuGet.Versioning.VersionRange;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class AlignedVersionsDownloader : IAlignedVersionsDownloader
    {
        private readonly IPackageRangeDownloader _packageRangeDownloader;

        public AlignedVersionsDownloader(IPackageRangeDownloader packageRangeDownloader)
        {
            _packageRangeDownloader = packageRangeDownloader;
        }

        public async Task<IEnumerable<NuGetVersion>> GetDownloadedVersionsAsync(
            IEnumerable<string> ids,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            return await GetAlignedVersionsAsync(
                ids,
                async id =>
                {
                    var identities = await _packageRangeDownloader.GetDownloadedVersionsAsync(
                        id,
                        sourceCacheContext,
                        log,
                        token);

                    return identities.Select(x => x.Version);
                });
        }

#if NETCOREAPP
        public async Task<IEnumerable<NuGetVersion>> DownloadPackagesAsync(
            IEnumerable<string> sources,
            IEnumerable<string> ids,
            NuGetVersionRange versionRange,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            var versions = await GetAvailableVersionsAsync(sources, ids, log, token);

            var limitedVersions = versions
                .Where(x => versionRange.Satisfies(x));

            var identities = ids
                .SelectMany(id => limitedVersions.Select(version => new PackageIdentity(id, version)));

            await _packageRangeDownloader.DownloadPackagesAsync(
                sources,
                identities,
                sourceCacheContext,
                log,
                token);

            return versions;
        }

        private async Task<IEnumerable<NuGetVersion>> GetAvailableVersionsAsync(
            IEnumerable<string> sources,
            IEnumerable<string> ids,
            ILogger log,
            CancellationToken token)
        {
            return await GetAlignedVersionsAsync(
                ids,
                async id =>
                {
                    var availableVersions = await _packageRangeDownloader.GetAvailableVersionsAsync(
                        sources,
                        id,
                        log,
                        token);

                    return availableVersions.Select(x => x.Version);
                });
        }
#endif

        private async Task<IEnumerable<NuGetVersion>> GetAlignedVersionsAsync(
            IEnumerable<string> ids,
            Func<string, Task<IEnumerable<NuGetVersion>>> getVersionsAsync)
        {
            var versions = new HashSet<NuGetVersion>();

            foreach (var id in ids)
            {
                var availableVersions = await getVersionsAsync(id);

                if (versions.Count == 0)
                {
                    versions.UnionWith(availableVersions);
                }
                else
                {
                    versions.IntersectWith(availableVersions);
                }
            }

            return versions;
        }
    }
}
