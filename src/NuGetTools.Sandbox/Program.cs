using System;
using System.IO;
using System.Linq;
using System.Threading;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;
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
                id: "NuGet.Versioning",
                versionRange: VersionRange.Parse("*"),
                log: NullLogger.Instance,
                token: CancellationToken.None).Wait();
            versionDownloader.DownloadAllVersions(
                id: "NuGet.Frameworks",
                versionRange: VersionRange.Parse("*"),
                log: NullLogger.Instance,
                token: CancellationToken.None).Wait();

            using (var packageLoader = new PackageLoader(nuGetSettings))
            {
                packageLoader.LoadPackageAssemblies(
                    "3.5.0-beta2-1484",
                    NuGetFramework.Parse("net46"),
                    new PackageIdentity("NuGet.Versioning", NuGetVersion.Parse("3.5.0-beta2-1484")));

                var context = packageLoader.LoadPackageAssemblies(
                    "3.5.0-beta2-1484",
                    NuGetFramework.Parse("net46"),
                    new PackageIdentity("NuGet.Frameworks", NuGetVersion.Parse("3.5.0-beta2-1484")));

                var frameworksAssemblyName = context.LoadedAssemblies.First(x => x.Name == "NuGet.Frameworks");
                var frameworkLogic = context.Proxy.GetFrameworkLogic(frameworksAssemblyName);
                var framework = frameworkLogic.Parse("net45");
                var frameworkShortFolderName = framework.ShortFolderName;

                var versioningAssemblyName = context.LoadedAssemblies.First(x => x.Name == "NuGet.Versioning");
                var versionLogic = context.Proxy.GetVersionLogic(versioningAssemblyName);
                var version = versionLogic.Parse("1.0.0-beta2+foo");
                var versionFullString = version.FullString;
                
                var versionRangeLogic = context.Proxy.GetVersionRangeLogic(versioningAssemblyName);
                var versionRange = versionRangeLogic.Parse("(, 1.0.0-beta]");
                var versionRangeNormalizedString = versionRange.NormalizedString;
            }
        }
    }
}
