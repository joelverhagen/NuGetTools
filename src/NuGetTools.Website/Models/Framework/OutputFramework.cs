using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class OutputFramework
    {
        public string Input { get; set; }
        public NuGetFramework Framework { get; set; }
        public bool Compatible { get; set; }
    }
}
