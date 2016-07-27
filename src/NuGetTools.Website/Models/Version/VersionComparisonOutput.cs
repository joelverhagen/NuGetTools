using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Website
{
    public class VersionComparisonOutput
    {
        public InputStatus InputStatus { get; set; }
        public VersionComparisonInput Input { get; set; }
        public bool IsVersionAValid { get; set; }
        public bool IsVersionBValid { get; set; }
        public IVersion VersionA { get; set; }
        public IVersion VersionB { get; set; }
        public ComparisonResult Result  { get; set; }
    }
}