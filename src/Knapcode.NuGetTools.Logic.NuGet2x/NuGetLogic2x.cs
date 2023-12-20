using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.NuGet2x
{
    public class NuGetLogic2x : INuGetLogic
    {
        public NuGetLogic2x()
        {
            Framework = new FrameworkLogic2x();
            Version = new VersionLogic2x();
            VersionRange = new VersionRangeLogic2x();
        }

        public IFrameworkLogic Framework { get; }
        public IVersionLogic Version { get; }
        public IVersionRangeLogic VersionRange { get; }
    }
}
