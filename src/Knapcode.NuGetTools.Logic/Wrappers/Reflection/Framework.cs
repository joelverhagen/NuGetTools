using System;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class Framework : MarshalByRefObject, IFramework
    {
        private readonly FrameworkApi _api;

        public Framework(object nuGetFramework, FrameworkApi api)
        {
            NuGetFramework = nuGetFramework;
            _api = api;
        }

        public object NuGetFramework { get; }
        public string DotNetFrameworkName => _api.GetDotNetFrameworkName(NuGetFramework);
        public string ShortFolderName => _api.GetShortFolderName(NuGetFramework);
        public string Identifier => _api.GetIdentifer(NuGetFramework);
        public System.Version Version => _api.GetVersion(NuGetFramework);
        public string Profile => _api.GetProfile(NuGetFramework);
    }
}
