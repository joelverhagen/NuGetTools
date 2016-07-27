using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Website
{
    public class OutputVersion
    {
        public string Input { get; set; }
        public IVersion Version { get; set; }
        public bool Satisfies { get; set; }
    }
}
