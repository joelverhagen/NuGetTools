using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.NuGet3x;

public class VersionRangeLogic3x : IVersionRangeLogic
{
    public bool FindBestMatchAvailable => true;
    public bool IsBetterAvailable => true;

    public IVersion? FindBestMatch(IVersionRange versionRange, IEnumerable<IVersion> versions)
    {
        var bestMatch = ((VersionRange3x)versionRange).VersionRange.FindBestMatch(
            versions.Cast<Version3x>().Select(x => x.NuGetVersion));

        if (bestMatch == null)
        {
            return null;
        }

        return versions.First(x => ReferenceEquals(((Version3x)x).NuGetVersion, bestMatch));
    }

    public bool IsBetter(IVersionRange versionRange, IVersion current, IVersion considering)
    {
        return ((VersionRange3x)versionRange).VersionRange.IsBetter(((Version3x)current).NuGetVersion, ((Version3x)considering).NuGetVersion);
    }

    public IVersionRange Parse(string input)
    {
        var nuGetVersionRange = VersionRange.Parse(input);
        return new VersionRange3x(nuGetVersionRange);
    }

    public bool Satisfies(IVersionRange versionRange, IVersion version)
    {
        return ((VersionRange3x)versionRange).VersionRange.Satisfies(((Version3x)version).NuGetVersion);
    }
}
