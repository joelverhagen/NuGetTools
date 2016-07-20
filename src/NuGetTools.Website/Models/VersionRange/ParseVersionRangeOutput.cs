using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class ParseVersionRangeOutput
    {
        public InputStatus InputStatus { get; set; }
        public ParseVersionRangeInput Input { get; set; }
        public VersionRange VersionRange { get; set; }
    }
}