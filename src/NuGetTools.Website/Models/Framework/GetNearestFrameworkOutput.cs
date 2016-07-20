using System.Collections.Generic;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class GetNearestFrameworkOutput
    {
        public InputStatus InputStatus { get; set; }
        public GetNearestFrameworkInput Input { get; set; }
        public bool IsProjectValid { get; set; }
        public bool IsPackageValid { get; set; }
        public NuGetFramework Project { get; set; }
        public IReadOnlyList<InputFramework> Package { get; set; }
        public InputFramework Nearest { get; set; }
        public IReadOnlyList<string> Invalid { get; set; }
    }
}