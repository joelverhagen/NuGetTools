using System;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class ReflectionVersionLogic : MarshalByRefObject, IVersionLogic<ReflectionVersion>
    {
        private readonly VersionApi _api;

        public ReflectionVersionLogic(VersionApi api)
        {
            _api = api;
        }

        public int Compare(ReflectionVersion versionA, ReflectionVersion versionB)
        {
            return _api.Compare(versionA.NuGetVersion, versionB.NuGetVersion);
        }

        public ReflectionVersion Parse(string input)
        {
            var nuGetVersion = _api.Parse(input);

            return new ReflectionVersion(nuGetVersion, _api);
        }
    }
}
