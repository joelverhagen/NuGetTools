using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x;

public class VersionRangeLogic2x : IVersionRangeLogic
{
    public bool FindBestMatchAvailable => false;
    public bool IsBetterAvailable => false;

    public IVersion FindBestMatch(IVersionRange versionRange, IEnumerable<IVersion> versions)
    {
        throw new NotSupportedException();
    }

    public bool IsBetter(IVersionRange versionRange, IVersion current, IVersion considering)
    {
        throw new NotSupportedException();
    }

    public IVersionRange Parse(string input)
    {
        return new VersionRange2x(VersionUtility.ParseVersionSpec(input));
    }

    public bool Satisfies(IVersionRange versionRange, IVersion version)
    {
        return ((VersionRange2x)versionRange).VersionSpec.Satisfies(((Version2x)version).SemanticVersion);
    }
}
