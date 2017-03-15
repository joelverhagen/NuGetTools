using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic.Direct;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Versioning;
using VersionRange = NuGet.Versioning.VersionRange;

namespace Knapcode.NuGetTools.PackageDownloader
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();
            
            var packagesDirectoryArgument = app.Argument(
                "packagesDirectory",
                "The directory to place all of the packages.");
            
            var sourceOption = app.Option(
                "--source",
                "The source to download packages from.",
                CommandOptionType.MultipleValue);

            app.OnExecute(() =>
            {
                if (string.IsNullOrWhiteSpace(packagesDirectoryArgument.Value))
                {
                    app.ShowHelp();
                    return 1;
                }
                
                var packagesDirectory = packagesDirectoryArgument.Value;
                List<string> sources;
                if (sourceOption.HasValue())
                {
                    sources = sourceOption.Values;
                }
                else
                {
                    sources = new List<string> { "https://api.nuget.org/v3/index.json" };
                }

                DownloadPackages(packagesDirectory, sources);

                return 0;
            });

            return app.Execute(args);
        }

        private static void DownloadPackages(string packagesDirectory, List<string> sources)
        {
            var settings = new InMemorySettings();
            var nuGetSettings = new NuGetSettings(settings);
            nuGetSettings.GlobalPackagesFolder = packagesDirectory;

            var packageRangeDownloader = new PackageRangeDownloader(nuGetSettings);

            var alignedVersionDownloader = new AlignedVersionsDownloader(packageRangeDownloader);

            var downloadTask = DownloadAsync(sources, alignedVersionDownloader);

            downloadTask.Wait();
        }

        private static async Task DownloadAsync(List<string> sources, AlignedVersionsDownloader alignedVersionDownloader)
        {
            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                ToolsFactory.PackageIds2x,
                VersionRange.Parse("[2.5.0, )", allowFloating: false),
                new ConsoleLogger(),
                CancellationToken.None);

            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                ToolsFactory.PackageIds3x,
                VersionRange.All,
                new ConsoleLogger(),
                CancellationToken.None);
        }
    }
}
