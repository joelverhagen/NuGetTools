using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.VersionRange;

public class OutputVersion
{
    public required string Input { get; set; }
    public required IVersion Version { get; set; }
    public bool Satisfies { get; set; }
}
