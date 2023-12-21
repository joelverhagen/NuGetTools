using System.Globalization;
using System.Reflection;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website;

public static class Configuration
{
    static Configuration()
    {
        var assembly = typeof(Configuration).GetTypeInfo().Assembly!;

        AssemblyVersion = assembly
            .GetCustomAttribute<AssemblyVersionAttribute>()?
            .Version!;

        AssemblyFileVersion = assembly
            .GetCustomAttribute<AssemblyFileVersionAttribute>()?
            .Version!;

        AssemblyInformationalVersion = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion!;

        if (SemanticVersion.TryParse(AssemblyInformationalVersion, out var version))
        {
            AssemblyCommitHash = version.Metadata;
            var shortCommitHash = version.Metadata?.Length > 8 ? version.Metadata.Substring(0, 8) : version.Metadata;
            AssemblyInformationalVersion = new SemanticVersion(version.Major, version.Minor, version.Patch, version.ReleaseLabels, shortCommitHash).ToFullString();
        }

        AssemblyBuildTimestamp = DateTimeOffset.Parse(assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => x.Key == "BuildTimestamp")?
            .Value ?? "2001-01-01", CultureInfo.InvariantCulture);
    }

    public static string AssemblyVersion { get; private set; }
    public static string AssemblyFileVersion { get; private set; }
    public static string AssemblyInformationalVersion { get; private set; }
    public static string? AssemblyCommitHash { get; private set; }
    public static DateTimeOffset AssemblyBuildTimestamp { get; private set; }
}
