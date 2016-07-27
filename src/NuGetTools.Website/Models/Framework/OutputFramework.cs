using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Website
{
    public class OutputFramework
    {
        public string Input { get; set; }
        public IFramework Framework { get; set; }
        public bool IsCompatible { get; set; }
    }
}
