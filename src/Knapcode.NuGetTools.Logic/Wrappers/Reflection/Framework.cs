using System;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class Framework : MarshalByRefObject, IFramework
    {
        private readonly IFrameworkApi _api;

        public Framework(object nuGetFramework, IFrameworkApi api)
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
        public bool HasProfile => _api.HasProfile(NuGetFramework);
        public bool HasPlatform => _api.HasPlatform(NuGetFramework);
        public string Platform => _api.GetPlatform(NuGetFramework);
        public System.Version PlatformVersion => _api.GetPlatformVersion(NuGetFramework);
        public string ToStringResult => _api.GetToStringResult(NuGetFramework);
    }
}
