using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework
{
    public class OutputFramework
    {
        public required string Input { get; set; }
        public required IFramework Framework { get; set; }
        public bool IsCompatible { get; set; }
    }
}
