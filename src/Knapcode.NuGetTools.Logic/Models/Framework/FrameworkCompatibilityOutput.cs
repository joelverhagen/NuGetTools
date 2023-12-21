using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework;

public class FrameworkCompatibilityOutput
{
    public InputStatus InputStatus { get; set; }
    public FrameworkCompatibilityInput? Input { get; set; }
    public bool IsProjectValid { get; set; }
    public bool IsPackageValid { get; set; }
    public IFramework? Project { get; set; }
    public IFramework? Package { get; set; }
    public bool IsCompatible { get; set; }
}