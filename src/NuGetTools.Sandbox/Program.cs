using System.IO;
using System.Linq;
using System.Threading;
using Knapcode.NuGetTools.Logic.Direct;
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

            var packageRangeDownloader = new PackageRangeDownloader(nuGetSettings);

            var alignedVersionDownloader = new AlignedVersionsDownloader(packageRangeDownloader);
            
            alignedVersionDownloader.DownloadPackagesAsync(
                new[] { "https://api.nuget.org/v3/index.json" },
                ToolsFactory.PackageIds,
                VersionRange.All,
                new ConsoleLogger(),
                CancellationToken.None).Wait();

            using (var assemblyLoader = new AssemblyLoader())
            using (var packageLoader = new PackageLoader(assemblyLoader, nuGetSettings))
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
