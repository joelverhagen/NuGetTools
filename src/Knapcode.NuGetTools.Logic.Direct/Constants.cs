namespace Knapcode.NuGetTools.Logic.Direct;

public static class Constants
{
    public const string VersioningId = "NuGet.Versioning";
    public const string FrameworksId = "NuGet.Frameworks";
    public const string CoreId = "NuGet.Core";

    public static IReadOnlyList<string> PackageIds3x { get; } = new[]
    {
        VersioningId,
        FrameworksId
    };

    public static IReadOnlyList<string> PackageIds2x { get; } = new[]
    {
        CoreId
    };
}
