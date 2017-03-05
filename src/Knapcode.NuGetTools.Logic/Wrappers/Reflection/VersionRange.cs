using System;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class VersionRange : MarshalByRefObject, IVersionRange
    {
        private readonly VersionRangeApi _api;
        private readonly VersionApi _versionApi;

        public VersionRange(object nuGetVersionRange, VersionApi versionApi, VersionRangeApi api)
        {
            NuGetVersionRange = nuGetVersionRange;
            _versionApi = versionApi;
            _api = api;
        }

        public object NuGetVersionRange { get; }
        public bool HasLowerBound => _api.HasLowerBound(NuGetVersionRange);
        public bool HasUpperBound => _api.HasUpperBound(NuGetVersionRange);
        public bool IsFloating => _api.IsFloating(NuGetVersionRange);
        public bool IsMaxInclusive => _api.IsMaxInclusive(NuGetVersionRange);
        public bool IsMinInclusive => _api.IsMinInclusive(NuGetVersionRange);
        public string NormalizedString => _api.GetNormalizedString(NuGetVersionRange);
        public string PrettyPrint => _api.PrettyPrint(NuGetVersionRange);
        public string LegacyShortString => _api.GetLegacyShortString(NuGetVersionRange);
        public string LegacyString => _api.GetLegacyString(NuGetVersionRange);
        public string OriginalString => _api.GetOriginalString(NuGetVersionRange);
        public bool LegacyShortStringAvailable => _api.GetLegacyShortStringAvailable();

        public IVersion MaxVersion
        {
            get
            {
                var nuGetVersion = _api.GetMaxVersion(NuGetVersionRange);

                if (nuGetVersion == null)
                {
                    return null;
                }

                return new Version(nuGetVersion, _versionApi);
            }
        }

        public IVersion MinVersion
        {
            get
            {
                var nuGetVersion = _api.GetMinVersion(NuGetVersionRange);

                if (nuGetVersion == null)
                {
                    return null;
                }

                return new Version(nuGetVersion, _versionApi);
            }
        }
    }
}
