using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.PackageManagement;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class PackageRangeDownloader : IPackageRangeDownloader
    {
        private readonly NuGetSettings _nuGetSettings;

        private readonly ConcurrentDictionary<string, SourceRepository> _sourceRepositories
            = new ConcurrentDictionary<string, SourceRepository>();

        private readonly ConcurrentDictionary<SourceRepository, Task<PackageMetadataResource>> _packageMetadataResources
            = new ConcurrentDictionary<SourceRepository, Task<PackageMetadataResource>>();

        private readonly Lazy<Task<FindPackageByIdResource>> _findPackageByIdResource; 

        public PackageRangeDownloader(NuGetSettings nuGetSettings)
        {
            _nuGetSettings = nuGetSettings;
            _findPackageByIdResource = new Lazy<Task<FindPackageByIdResource>>(GetFindPackageByIdResourceAsync);
        }

        public async Task<IEnumerable<PackageIdentity>> GetDownloadedVersionsAsync(
            string id,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            var findPackageByIdResource = await _findPackageByIdResource.Value;

            var versions = await findPackageByIdResource.GetAllVersionsAsync(id, sourceCacheContext, log, token);

            return versions.Select(x => new PackageIdentity(id, x));
        }

        private async Task<FindPackageByIdResource> GetFindPackageByIdResourceAsync()
        {
            var sourceRepository = GetSourceRepository(_nuGetSettings.GlobalPackagesFolder);

            var findPackageByIdResource = await sourceRepository
                .GetResourceAsync<FindPackageByIdResource>(CancellationToken.None);

            return findPackageByIdResource;
        }

        public async Task<IEnumerable<PackageIdentity>> GetAvailableVersionsAsync(
            IEnumerable<string> sources,
            string id,
            ILogger log,
            CancellationToken token)
        {
            var sourceRepositories = GetSourceRepositories(sources);

            var versionTasks = sourceRepositories
                .Select(x => GetAvailableVersionsAsync(x, id, log, token));

            var versionSets = await Task.WhenAll(versionTasks);

            return versionSets
                .SelectMany(x => x)
                .OrderBy(x => x)
                .Distinct()
                .ToArray();
        }

        public async Task DownloadPackagesAsync(
            IEnumerable<string> sources,
            IEnumerable<PackageIdentity> packageIdentities,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            var sourceRepositories = GetSourceRepositories(sources);
            
            var downloadTasks = packageIdentities
                .Select(x => DownloadPackageAsync(sourceRepositories, x, sourceCacheContext, log, token));

            await Task.WhenAll(downloadTasks);
        }

        private List<SourceRepository> GetSourceRepositories(IEnumerable<string> sources)
        {
            return sources.Select(GetSourceRepository).ToList();
        }

        private SourceRepository GetSourceRepository(string source)
        {
            return _sourceRepositories.GetOrAdd(
                source,
                key => Repository.Factory.GetCoreV3(key));
        }

        private async Task<IEnumerable<PackageIdentity>> GetAvailableVersionsAsync(
            SourceRepository sourceRepository,
            string id,
            ILogger log,
            CancellationToken token)
        {
            var metadataResource = await _packageMetadataResources.GetOrAdd(
                sourceRepository,
                key => key.GetResourceAsync<PackageMetadataResource>(token));
            
            var allMetadata = await metadataResource.GetMetadataAsync(
                id,
                includePrerelease: true,
                includeUnlisted: false,
                log: log,
                token: token);

            return allMetadata.Select(x => x.Identity);
        }

        private async Task DownloadPackageAsync(
            List<SourceRepository> sourceRepositories,
            PackageIdentity identity,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            var packageDownloadContext = new PackageDownloadContext(sourceCacheContext);
            var result = await PackageDownloader.GetDownloadResourceResultAsync(
                sourceRepositories,
                packageIdentity: identity,
                downloadContext: packageDownloadContext,
                globalPackagesFolder: _nuGetSettings.GlobalPackagesFolder,
                logger: log,
                token: token);

            using (result)
            {
                if (result.Status != DownloadResourceResultStatus.Available)
                {
                    throw new InvalidOperationException($"The package '{identity}' is not available.");
                }
            }
        }
    }
}
