using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic.Direct;
using Microsoft.Extensions.CommandLineUtils;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
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

            var versionFileOption = app.Option(
                "--version-file",
                "A text file to write all package version to after completion.",
                CommandOptionType.SingleValue);

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

                var downloadTask = DownloadPackagesAsync(packagesDirectory, sources, CancellationToken.None);
                var versions = downloadTask.GetAwaiter().GetResult();

                if (versionFileOption.HasValue())
                {
                    File.WriteAllLines(versionFileOption.Value(), versions.Select(x => x.ToNormalizedString()));
                }

                return 0;
            });

            return app.Execute(args);
        }

        private static async Task<List<NuGetVersion>> DownloadPackagesAsync(string packagesDirectory, List<string> sources, CancellationToken token)
        {
            var settings = new InMemorySettings();
            var nuGetSettings = new NuGetSettings(settings);
            nuGetSettings.GlobalPackagesFolder = packagesDirectory;

            var packageRangeDownloader = new PackageRangeDownloader(nuGetSettings);
            var downloader = new AlignedVersionsDownloader(packageRangeDownloader);

            using (var cacheContext = new SourceCacheContext())
            {
                var logger = new ConsoleLogger();

                await DownloadAsync(sources, downloader, cacheContext, logger, token);

                var versions2x = await downloader.GetDownloadedVersionsAsync(
                    Constants.PackageIds2x,
                    cacheContext,
                    logger,
                    token);

                var versions3x = await downloader.GetDownloadedVersionsAsync(
                    Constants.PackageIds3x,
                    cacheContext,
                    logger,
                    token);

                return versions2x
                    .Concat(versions3x)
                    .Distinct()
                    .OrderBy(v => v)
                    .ToList();
            }
        }

        private static async Task DownloadAsync(
            List<string> sources,
            AlignedVersionsDownloader alignedVersionDownloader,
            SourceCacheContext sourceCacheContext,
            ILogger log,
            CancellationToken token)
        {
            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                Constants.PackageIds2x,
                VersionRange.Parse("[2.5.0, )", allowFloating: false),
                sourceCacheContext,
                log,
                token);

            await alignedVersionDownloader.DownloadPackagesAsync(
                sources,
                Constants.PackageIds3x,
                VersionRange.All,
                sourceCacheContext,
                log,
                token);
        }
    }
}
