using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Logic.Direct.Wrappers
{
    public class Framework : IFramework
    {
        public Framework(NuGetFramework framework)
        {
            NuGetFramework = framework;
        }

        public NuGetFramework NuGetFramework { get; }
        public string DotNetFrameworkName => NuGetFramework.DotNetFrameworkName;
        public string ShortFolderName => NuGetFramework.GetShortFolderName();
        public string Identifier => NuGetFramework.Framework;
        public System.Version Version => NuGetFramework.Version;
        public string Profile => NuGetFramework.Profile;
    }
}
