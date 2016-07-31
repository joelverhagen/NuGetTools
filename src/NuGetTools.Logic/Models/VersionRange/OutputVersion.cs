using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.VersionRange
{
    public class OutputVersion
    {
        public string Input { get; set; }
        public IVersion Version { get; set; }
        public bool Satisfies { get; set; }
    }
}
