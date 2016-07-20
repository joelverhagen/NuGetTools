using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Website
{
    public class ParseFrameworkOutput
    {
        public InputStatus InputStatus { get; set; }
        public ParseFrameworkInput Input { get; set; }
        public NuGetFramework Framework { get; set; }
    }
}