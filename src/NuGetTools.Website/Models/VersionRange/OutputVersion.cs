using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class OutputVersion
    {
        public string Input { get; set; }
        public NuGetVersion Version { get; set; }
        public bool Satisfies { get; set; }
    }
}
