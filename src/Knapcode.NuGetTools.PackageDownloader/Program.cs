using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic.Direct;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
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

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var downloadTask = DownloadAsync(
                    sources,
                    alignedVersionDownloader,
                    sourceCacheContext,
                    new ConsoleLogger());

                downloadTask.Wait();
            }
        }

        private static async Task DownloadAsync(
            List<string> sources,
            AlignedVersionsDownloader alignedVersionDownloader,
            SourceCacheContext sourceCacheContext,
            ILogger log)
        {
            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                ToolsFactory.PackageIds2x,
                VersionRange.Parse("[2.5.0, )", allowFloating: false),
                sourceCacheContext,
                log,
                CancellationToken.None);

            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                ToolsFactory.PackageIds3x,
                VersionRange.All,
                sourceCacheContext,
                log,
                CancellationToken.None);
        }
    }
}
