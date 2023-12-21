namespace Knapcode.NuGetTools.Logic.Wrappers;

public static class VersionExtensions
{
    public static string GetPrettyString(this IVersion version)
    {
        if (version.FullStringAvailable)
        {
            return version.FullString;
        }

        if (version.NormalizedStringAvailable)
        {
            return version.NormalizedString;
        }

        return version.ToStringResult;
    }
}
