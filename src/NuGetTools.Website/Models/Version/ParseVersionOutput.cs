using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class ParseVersionOutput
    {
        public InputStatus InputStatus { get; set; }
        public ParseVersionInput Input { get; set; }
        public NuGetVersion Version { get; set; }
    }
}