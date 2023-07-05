using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic.Direct;
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
            var rootCommand = new RootCommand
            {
                GetDownloadCommand("download"),
                GetCheckVersionsCommand("check-versions"),
            };

            return rootCommand.Invoke(args);
        }

        private static Command GetDownloadCommand(string name)
        {
            var command = new Command(name, "Download NuGet client SDK packages to a specific directory.");

            var packagesDirectoryArgument = new Argument<string>("PACKAGES_DIR", "The directory to place all of the packages.");
            command.AddArgument(packagesDirectoryArgument);

            var sourceOption = new Option<List<string>>(
                "--source",
                getDefaultValue: () => new List<string> { "https://api.nuget.org/v3/index.json" },
                "The source to download packages from. Multiple can be provided.")
            {
                Arity = ArgumentArity.ZeroOrMore,
            };
            command.AddOption(sourceOption);

            command.SetHandler(async context =>
            {
                var packagesDirectory = context.ParseResult.GetValueForArgument(packagesDirectoryArgument);
                var sources = context.ParseResult.GetValueForOption(sourceOption);

                await DownloadPackagesAsync(packagesDirectory, sources, context.GetCancellationToken());
            });

            return command;
        }

        private static async Task DownloadPackagesAsync(string packagesDirectory, List<string> sources, CancellationToken token)
        {
            var downloader = GetDownloader(packagesDirectory);

            using (var cacheContext = new SourceCacheContext())
            {
                var logger = new ConsoleLogger();

                await DownloadAsync(sources, downloader, cacheContext, logger, token);
            }
        }

        private static AlignedVersionsDownloader GetDownloader(string packagesDirectory)
        {
            var settings = new InMemorySettings();
            var nuGetSettings = new NuGetSettings(settings);
            nuGetSettings.GlobalPackagesFolder = packagesDirectory;

            var packageRangeDownloader = new PackageRangeDownloader(nuGetSettings);
            var downloader = new AlignedVersionsDownloader(packageRangeDownloader);
            return downloader;
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

        private static Command GetCheckVersionsCommand(string name)
        {
            var command = new Command(name, "Compares the list of package versions locally with a deployed NuGetTools web app.");

            var packagesDirectoryArgument = new Argument<string>("PACKAGES_DIR", "The directory to check for downloaded packages.");
            command.AddArgument(packagesDirectoryArgument);

            var baseUrlArgument = new Argument<string>("BASE_URL", "The base URL for the NuGetTools web app.");
            command.AddArgument(baseUrlArgument);

            command.SetHandler(async context =>
            {
                var packagesDirectory = context.ParseResult.GetValueForArgument(packagesDirectoryArgument);
                var baseUrl = context.ParseResult.GetValueForArgument(baseUrlArgument);

                await CheckVersionsAsync(packagesDirectory, baseUrl, context.GetCancellationToken());
            });

            return command;
        }

        private static async Task CheckVersionsAsync(string packagesDirectory, string baseUrl, CancellationToken token)
        {
            var downloader = GetDownloader(packagesDirectory);

            HashSet<NuGetVersion> remoteVersions;
            using (var httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) })
            {
                var json = await httpClient.GetStringAsync("api/versions");
                remoteVersions = JsonSerializer.Deserialize<List<string>>(json).Select(NuGetVersion.Parse).ToHashSet();
            }

            HashSet<NuGetVersion> localVersions;
            using (var cacheContext = new SourceCacheContext())
            {
                var logger = new ConsoleLogger();

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

                localVersions = versions2x.Concat(versions3x).ToHashSet();
            }

            var differentVersions = remoteVersions
                .Union(localVersions)
                .Select(v => new
                {
                    Version = v,
                    IsRemote = remoteVersions.Contains(v),
                    IsLocal = localVersions.Contains(v),
                })
                .OrderBy(v => v.Version)
                .ToList();

            foreach (var version in differentVersions)
            {
                if (!version.IsRemote)
                {
                    Console.WriteLine("Different (local only): " + version.Version.ToNormalizedString());
                }
                else if (!version.IsLocal)
                {
                    Console.WriteLine("Different (remote only): " + version.Version.ToNormalizedString());
                }
                else
                {
                    Console.WriteLine("Matching (local and remote): " + version.Version.ToNormalizedString());
                }
            }
        }
    }
}
