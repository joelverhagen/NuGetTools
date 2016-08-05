using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public interface IAlignedVersionsDownloader
    {
        Task<IEnumerable<NuGetVersion>> GetDownloadedVersionsAsync(IEnumerable<string> ids, CancellationToken token);
        Task<IEnumerable<NuGetVersion>> DownloadPackagesAsync(IEnumerable<string> sources, IEnumerable<string> ids, VersionRange versionRange, ILogger log, CancellationToken token);
    }
}