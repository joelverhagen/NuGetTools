using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Logic.Wrappers.Direct
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
    }
}
