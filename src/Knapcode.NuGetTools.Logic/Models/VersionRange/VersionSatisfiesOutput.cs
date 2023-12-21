using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.VersionRange;

public class VersionSatisfiesOutput
{
    public InputStatus InputStatus { get; set; }
    public VersionSatisfiesInput? Input { get; set; }
    public bool IsVersionRangeValid { get; set; }
    public bool IsVersionValid { get; set; }
    public IVersionRange? VersionRange { get; set; }
    public IVersion? Version { get; set; }
    public bool Satisfies { get; set; }
}
