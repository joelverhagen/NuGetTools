using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class VersionComparisonOutput
    {
        public InputStatus InputStatus { get; set; }
        public VersionComparisonInput Input { get; set; }
        public bool IsVersionAValid { get; set; }
        public bool IsVersionBValid { get; set; }
        public NuGetVersion VersionA { get; set; }
        public NuGetVersion VersionB { get; set; }
        public ComparisonResult Result  { get; set; }
    }
}