using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public interface IAlignedVersionsDownloader
    {
        Task<IEnumerable<NuGetVersion>> GetDownloadedVersionsAsync(
            IEnumerable<string> ids,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token);

        Task<IEnumerable<NuGetVersion>> DownloadPackagesAsync(
            IEnumerable<string> sources,
            IEnumerable<string> ids,
            VersionRange versionRange,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token);
    }
}