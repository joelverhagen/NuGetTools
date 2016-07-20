using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class InputFramework
    {
        public string Input { get; set; }
        public NuGetFramework Framework { get; set; }
        public bool Compatible { get; set; }
    }
}
