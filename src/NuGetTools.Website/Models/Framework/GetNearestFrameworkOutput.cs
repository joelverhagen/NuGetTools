using System.Collections.Generic;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Website
{
    public class GetNearestFrameworkOutput
    {
        public InputStatus InputStatus { get; set; }
        public GetNearestFrameworkInput Input { get; set; }
        public bool IsProjectValid { get; set; }
        public bool IsPackageValid { get; set; }
        public IFramework Project { get; set; }
        public IReadOnlyList<OutputFramework> Package { get; set; }
        public OutputFramework Nearest { get; set; }
        public IReadOnlyList<string> Invalid { get; set; }
    }
}