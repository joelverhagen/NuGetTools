using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.NuGet3x
{
    public class VersionLogic3x : IVersionLogic
    {
        public int Compare(IVersion versionA, IVersion versionB)
        {
            return ((Version3x)versionA).NuGetVersion.CompareTo(((Version3x)versionB).NuGetVersion);
        }

        public IVersion Parse(string input)
        {
            var nuGetVersion = NuGetVersion.Parse(input);
            return new Version3x(nuGetVersion);
        }
    }
}
