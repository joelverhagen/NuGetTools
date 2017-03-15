using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic.Wrappers;
using NuGetVersionRange = NuGet.Versioning.VersionRange;

namespace Knapcode.NuGetTools.Logic.Direct.Wrappers
{
    public class VersionRangeLogic : IVersionRangeLogic<Version, VersionRange>
    {
        public bool FindBestMatchAvailable => true;

        public Version FindBestMatch(VersionRange versionRange, IEnumerable<Version> versions)
        {
            var bestMatch = versionRange.NuGetVersionRange.FindBestMatch(
                versions.Select(x => x.NuGetVersion));

            if (bestMatch == null)
            {
                return null;
            }

            return versions.First(x => ReferenceEquals(x.NuGetVersion, bestMatch));
        }

        public VersionRange Parse(string input)
        {
            var nuGetVersionRange = NuGetVersionRange.Parse(input);
            return new VersionRange(nuGetVersionRange);
        }

        public bool Satisfies(VersionRange versionRange, Version version)
        {
            return versionRange.NuGetVersionRange.Satisfies(version.NuGetVersion);
        }
    }
}
