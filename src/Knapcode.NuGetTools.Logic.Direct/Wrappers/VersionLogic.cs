using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Direct.Wrappers
{
    public class VersionLogic : IVersionLogic<Version>
    {
        public int Compare(Version versionA, Version versionB)
        {
            return versionA.NuGetVersion.CompareTo(versionB.NuGetVersion);
        }

        public Version Parse(string input)
        {
            var nuGetVersion = NuGetVersion.Parse(input);
            return new Version(nuGetVersion);
        }
    }
}
