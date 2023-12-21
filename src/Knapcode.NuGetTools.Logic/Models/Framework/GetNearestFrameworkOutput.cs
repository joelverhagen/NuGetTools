using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework;

public class GetNearestFrameworkOutput
{
    public InputStatus InputStatus { get; set; }
    public GetNearestFrameworkInput? Input { get; set; }
    public bool IsProjectValid { get; set; }
    public bool IsPackageValid { get; set; }
    public IFramework? Project { get; set; }
    public required IReadOnlyList<OutputFramework> Package { get; set; }
    public OutputFramework? Nearest { get; set; }
    public required IReadOnlyList<string> Invalid { get; set; }
}