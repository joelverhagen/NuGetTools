using System;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class VersionLogic : MarshalByRefObject, IVersionLogic<Version>
    {
        private readonly IVersionApi _api;

        public VersionLogic(IVersionApi api)
        {
            _api = api;
        }

        public int Compare(Version versionA, Version versionB)
        {
            return _api.Compare(versionA.NuGetVersion, versionB.NuGetVersion);
        }

        public Version Parse(string input)
        {
            var nuGetVersion = _api.Parse(input);

            return new Version(nuGetVersion, _api);
        }
        
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
