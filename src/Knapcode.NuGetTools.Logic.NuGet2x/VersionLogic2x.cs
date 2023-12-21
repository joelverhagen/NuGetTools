using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x;

public class VersionLogic2x : IVersionLogic
{
    public int Compare(IVersion versionA, IVersion versionB)
    {
        return ((Version2x)versionA).SemanticVersion.CompareTo(((Version2x)versionB).SemanticVersion);
    }

    public IVersion Parse(string input)
    {
        return new Version2x(SemanticVersion.Parse(input));
    }
}
