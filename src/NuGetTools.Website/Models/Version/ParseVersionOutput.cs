using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Website
{
    public class ParseVersionOutput
    {
        public InputStatus InputStatus { get; set; }
        public ParseVersionInput Input { get; set; }
        public IVersion Version { get; set; }
    }
}