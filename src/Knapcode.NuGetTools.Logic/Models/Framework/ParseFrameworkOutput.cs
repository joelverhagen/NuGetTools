using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework
{
    public class ParseFrameworkOutput
    {
        public InputStatus InputStatus { get; set; }
        public ParseFrameworkInput Input { get; set; }
        public IFramework Framework { get; set; }
    }
}