using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class ReflectionVersionRangeLogic : MarshalByRefObject, IVersionRangeLogic<ReflectionVersion, ReflectionVersionRange>
    {
        private readonly VersionRangeApi _api;
        private readonly VersionApi _versionApi;

        public ReflectionVersionRangeLogic(VersionApi versionApi, VersionRangeApi api)
        {
            _versionApi = versionApi;
            _api = api;
        }

        public ReflectionVersion FindBestMatch(ReflectionVersionRange versionRange, IEnumerable<ReflectionVersion> versions)
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

        public ReflectionVersionRange Parse(string input)
        {
            var nuGetVersionRange = _api.Parse(input);

            return new ReflectionVersionRange(nuGetVersionRange, _versionApi, _api);
        }

        public bool Satisfies(ReflectionVersionRange versionRange, ReflectionVersion version)
        {
            return _api.Satisfies(
                versionRange.NuGetVersionRange,
                version.NuGetVersion);
        }
    }
}
