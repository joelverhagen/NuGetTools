using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.VersionRange;

public class ParseVersionRangeOutput
{
    public InputStatus InputStatus { get; set; }
    public ParseVersionRangeInput? Input { get; set; }
    public IVersionRange? VersionRange { get; set; }
}