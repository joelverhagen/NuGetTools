using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class VersionSatisfiesOutput
    {
        public InputStatus InputStatus { get; set; }
        public VersionSatisfiesInput Input { get; set; }
        public bool IsVersionRangeValid { get; set; }
        public bool IsVersionValid { get; set; }
        public VersionRange VersionRange { get; set; }
        public NuGetVersion Version { get; set; }
        public bool Satisfies { get; set; }
    }
}