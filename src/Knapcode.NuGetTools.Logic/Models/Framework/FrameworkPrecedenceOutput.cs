using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework;

public class FrameworkPrecedenceOutput
{
    public required InputStatus InputStatus { get; set; }
    public required FrameworkPrecedenceInput Input { get; set; }
    public IFramework? Framework { get; set; }
    public required IReadOnlyList<IFramework> Precedence { get; set; }
}