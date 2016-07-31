using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework
{
    public class OutputFramework
    {
        public string Input { get; set; }
        public IFramework Framework { get; set; }
        public bool IsCompatible { get; set; }
    }
}
