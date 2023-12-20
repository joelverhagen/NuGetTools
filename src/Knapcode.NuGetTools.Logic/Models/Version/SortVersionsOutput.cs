using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Version
{
    public class SortVersionsOutput
    {
        public InputStatus InputStatus { get; set; }
        public SortVersionsInput? Input { get; set; }
        public required IReadOnlyList<IVersion> Versions { get; set; }
        public required IReadOnlyList<string> Invalid { get; set; }
    }
}
