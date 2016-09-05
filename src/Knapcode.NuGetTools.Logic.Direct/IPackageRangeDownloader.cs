using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Packaging.Core;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public interface IPackageRangeDownloader
    {
        Task<IEnumerable<PackageIdentity>> GetDownloadedVersionsAsync(string id, CancellationToken token);
        Task DownloadPackagesAsync(IEnumerable<string> sources, IEnumerable<PackageIdentity> packageIdentities, ILogger log, CancellationToken token);
        Task<IEnumerable<PackageIdentity>> GetAvailableVersionsAsync(IEnumerable<string> sources, string id, ILogger log, CancellationToken token);
    }
}