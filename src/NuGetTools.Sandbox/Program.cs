using System.IO;
using System.Threading;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Wrappers.Remote;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var settings = new InMemorySettings();
            var nuGetSettings = new NuGetSettings(settings);
            nuGetSettings.GlobalPackagesFolder = Path.GetFullPath("packages");

            var versionDownloader = new PackageDownloader(
                "https://api.nuget.org/v3/index.json",
                nuGetSettings);
            
            versionDownloader.DownloadAllVersions(
                id: "NuGet.Frameworks",
                versionRange: VersionRange.Parse("*"),
                log: NullLogger.Instance,
                token: CancellationToken.None).Wait();

            using (var packageLoader = new PackageLoader(nuGetSettings))
            {
                var assemblyNames = packageLoader.LoadPackageAssemblies(
                    "3.5.0-beta2-1484",
                    NuGetFramework.Parse("net46"),
                    new PackageIdentity("NuGet.Frameworks", NuGetVersion.Parse("3.5.0-beta2-1484")));
            }            
        }
    }
}
