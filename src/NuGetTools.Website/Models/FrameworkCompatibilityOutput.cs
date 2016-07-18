using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class FrameworkCompatibilityOutput
    {
        public InputStatus InputStatus { get; set; }
        public FrameworkCompatibilityInput Input { get; set; }
        public bool IsProjectValid { get; set; }
        public bool IsPackageValid { get; set; }
        public NuGetFramework Project { get; set; }
        public NuGetFramework Package { get; set; }
        public bool Compatible { get; set; }
    }
}