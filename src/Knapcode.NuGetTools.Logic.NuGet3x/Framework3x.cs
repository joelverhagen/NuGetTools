using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Logic.NuGet3x
{
    public class Framework3x : IFramework
    {
        public Framework3x(NuGetFramework framework)
        {
            NuGetFramework = framework;
        }

        public NuGetFramework NuGetFramework { get; }

        public string DotNetFrameworkName => NuGetFramework.DotNetFrameworkName;
        public string ShortFolderName => NuGetFramework.GetShortFolderName();
        public string Identifier => NuGetFramework.Framework;
        public System.Version Version => NuGetFramework.Version;
        public bool HasProfile => NuGetFramework.HasProfile;
        public string Profile => NuGetFramework.Profile;
        public bool HasPlatform => NuGetFramework.HasPlatform;
        public string Platform => NuGetFramework.Platform;
        public System.Version PlatformVersion => NuGetFramework.PlatformVersion;
        public string ToStringResult => NuGetFramework.ToString();
    }
}
