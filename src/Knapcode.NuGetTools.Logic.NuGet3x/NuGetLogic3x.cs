using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.NuGet3x;

public class NuGetLogic3x : INuGetLogic
{
    public NuGetLogic3x()
    {
        Framework = new FrameworkLogic3x();
        Version = new VersionLogic3x();
        VersionRange = new VersionRangeLogic3x();
    }

    public IFrameworkLogic Framework { get; }
    public IVersionLogic Version { get; }
    public IVersionRangeLogic VersionRange { get; }
}
