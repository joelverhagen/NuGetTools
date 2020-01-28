using System;
using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class VersionRangeLogic : MarshalByRefObject, IVersionRangeLogic<Version, VersionRange>
    {
        private readonly IVersionRangeApi _api;
        private readonly IVersionApi _versionApi;

        public bool FindBestMatchAvailable => _api.FindBestMatchAvailable();

        public bool IsBetterAvailable => _api.IsBetterAvailable();

        public VersionRangeLogic(IVersionApi versionApi, IVersionRangeApi api)
        {
            _versionApi = versionApi;
            _api = api;
        }

        public Version FindBestMatch(VersionRange versionRange, IEnumerable<Version> versions)
        {
            var nuGetVersions = versions.Select(x => x.NuGetVersion);

            var bestMatch = _api.FindBestMatch(
                versionRange.NuGetVersionRange,
                nuGetVersions);

            if (bestMatch == null)
            {
                return null;
            }

            return versions.First(x => ReferenceEquals(x.NuGetVersion, bestMatch));
        }

        public bool IsBetter(VersionRange versionRange, Version current, Version considering)
        {
            return _api.IsBetter(versionRange.NuGetVersionRange, current.NuGetVersion, considering.NuGetVersion);
        }

        public VersionRange Parse(string input)
        {
            var nuGetVersionRange = _api.Parse(input);

            return new VersionRange(nuGetVersionRange, _versionApi, _api);
        }

        public bool Satisfies(VersionRange versionRange, Version version)
        {
            return _api.Satisfies(
                versionRange.NuGetVersionRange,
                version.NuGetVersion);
        }
        
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
