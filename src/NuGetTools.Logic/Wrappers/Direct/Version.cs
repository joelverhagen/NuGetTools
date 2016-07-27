using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.Wrappers.Direct
{
    public class Version : IVersion
    {
        public Version(NuGetVersion version)
        {
            NuGetVersion = version;
        }

        public NuGetVersion NuGetVersion { get; }
        public string FullString => NuGetVersion.ToFullString();
        public bool IsPrerelease => NuGetVersion.IsPrerelease;
        public bool IsSemVer2 => NuGetVersion.IsSemVer2;
        public string NormalizedString => NuGetVersion.ToNormalizedString();
        public int Revision => NuGetVersion.Revision;
    }
}
