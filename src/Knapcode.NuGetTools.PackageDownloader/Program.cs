using System.CommandLine;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Knapcode.NuGetTools.Logic.Direct;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.PackageDownloader;

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

        var packagesDirectoryArgument = new Argument<string>("PACKAGES_DIR", "The directory to place all of the packages.")
        {
            Arity = ArgumentArity.ZeroOrOne,
        };
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
            var packagesArgument = (string?)context.ParseResult.GetValueForArgument(packagesDirectoryArgument);
            var packagesDirectory = Path.GetFullPath(packagesArgument ?? GetWebsitePackagesDirectory());
            var sources = context.ParseResult.GetValueForOption(sourceOption);

            if (sources is null)
            {
                return;
            }

            await DownloadPackagesAsync(packagesDirectory, sources, context.GetCancellationToken());
        });

        return command;
    }

    private static async Task DownloadPackagesAsync(string packagesDirectory, List<string> sources, CancellationToken token)
    {
        var downloader = GetDownloader(packagesDirectory);

        // download required NuGet.* packages
        using (var cacheContext = new SourceCacheContext())
        {
            var logger = new ConsoleLogger();

            await DownloadAsync(sources, downloader, cacheContext, logger, token);
        }

        // find all used assemblies
        var versions = await GetLocalVersionsAsync(downloader, token);
        var usedAssemblies = new HashSet<string>();
        foreach (var version in versions)
        {
            Console.WriteLine($"Loading {version}");
            var assemblies = await VersionedToolsFactory.GetLoadedAssembliesAsync(packagesDirectory, version);
            var paths = assemblies.Select(x => x.Location).Where(x => x is not null).ToList();
            usedAssemblies.UnionWith(paths);
        }

        // clean up extra file extensions
        var deleteExtensions = new[]
        {
            ".nupkg",
            ".nuspec",
            ".xml",
            ".png",
            ".md",
            ".signature.p7s",
            ".nupkg.metadata"
        };
        var allowedExtensions = new[]
        {
            ".nupkg.sha512",
        };

        var unexpected = 0;
        foreach (var path in Directory.EnumerateFiles(packagesDirectory, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(packagesDirectory, path);
            ConsoleColor color;

            var matchingAllowed = allowedExtensions.FirstOrDefault(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));
            if (matchingAllowed is not null)
            {
                color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"Leaving allowed *{matchingAllowed} at {relative}");
                Console.ForegroundColor = color;
                continue;
            }

            var matchingDelete = deleteExtensions.FirstOrDefault(x => path.EndsWith(x, StringComparison.OrdinalIgnoreCase));
            if (matchingDelete is not null)
            {
                Console.WriteLine($"Deleting *{matchingDelete} at {relative}");
                File.Delete(path);
                continue;
            }

            if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(path), ".dll"))
            {
                if (!usedAssemblies.Contains(path))
                {
                    Console.WriteLine($"Deleting unused DLL at {relative}");
                    File.Delete(path);
                }
                else
                {
                    color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"Leaving used DLL at {relative}");
                    Console.ForegroundColor = color;
                }

                continue;
            }

            color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Unexpected file at {relative}");
            Console.ForegroundColor = color;
            unexpected++;
        }

        if (unexpected > 0)
        {
            throw new InvalidOperationException($"There are {unexpected} files. Update the PackageDownloader project to account for this.");
        }

        foreach (var directory in Directory.EnumerateDirectories(packagesDirectory, "*", SearchOption.AllDirectories))
        {
            var current = directory;

            while (current is not null)
            {
                if (Directory.EnumerateFileSystemEntries(current).Any())
                {
                    break;
                }

                var relative = Path.GetRelativePath(packagesDirectory, current);
                Console.WriteLine($"Deleting empty directory at {relative}");
                Directory.Delete(current);

                current = Path.GetDirectoryName(current);
            }
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
            var versions = JsonSerializer.Deserialize<List<string>>(json);

            if (versions is null || versions.Count == 0)
            {
                throw new InvalidOperationException("No remote versions were found.");
            }

            remoteVersions = versions.Select(NuGetVersion.Parse).ToHashSet();
        }

        var localVersions = await GetLocalVersionsAsync(downloader, token);

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

    private static async Task<HashSet<NuGetVersion>> GetLocalVersionsAsync(AlignedVersionsDownloader downloader, CancellationToken token)
    {
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

        return localVersions;
    }

    private static string GetWebsitePackagesDirectory([CallerFilePath] string path = "")
    {
        return Path.Combine(Path.GetDirectoryName(path)!, "..", "Knapcode.NuGetTools.Website", "packages");
    }
}
