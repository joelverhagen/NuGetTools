using System;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.Core.v3;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class PackageDownloader
    {
        private readonly string _source;
        private readonly NuGetSettings _settings;
        private readonly Lazy<SourceRepository> _sourceRepository;

        public PackageDownloader(string source, NuGetSettings settings)
        {
            _source = source;
            _settings = settings;
            _sourceRepository = new Lazy<SourceRepository>(() => Repository.Factory.GetCoreV3(source));
        }

        public async Task DownloadAllVersions(string id, VersionRange versionRange, ILogger log, CancellationToken token)
        {
            var downloadResource = await _sourceRepository
                .Value
                .GetResourceAsync<DownloadResource>(token);

            var metadataResource = await _sourceRepository
                .Value
                .GetResourceAsync<PackageMetadataResource>(token);

            var allMetadata = await metadataResource.GetMetadataAsync(
                id,
                includePrerelease: true,
                includeUnlisted: false,
                log: log,
                token: token);

            foreach (var metadata in allMetadata)
            {
                if (!versionRange.Satisfies(metadata.Identity.Version))
                {
                    continue;
                }

                var download = await downloadResource.GetDownloadResourceResultAsync(
                    metadata.Identity,
                    _settings.Settings,
                    log,
                    token);

                using (download)
                {
                }
            }
        }
    }
}
