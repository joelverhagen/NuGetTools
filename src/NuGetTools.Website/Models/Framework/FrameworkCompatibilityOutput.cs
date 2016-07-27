using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class FrameworkCompatibilityOutput
    {
        public InputStatus InputStatus { get; set; }
        public FrameworkCompatibilityInput Input { get; set; }
        public bool IsProjectValid { get; set; }
        public bool IsPackageValid { get; set; }
        public IFramework Project { get; set; }
        public IFramework Package { get; set; }
        public bool IsCompatible { get; set; }
    }
}