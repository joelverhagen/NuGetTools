using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.NuGet3x
{
    public class VersionRange3x : IVersionRange
    {
        private static readonly bool StaticLegacyShortStringAvailable = typeof(VersionRange)
            .GetMethod(nameof(NuGet.Versioning.VersionRange.ToLegacyShortString)) is not null;

        public VersionRange3x(VersionRange versionRange)
        {
            VersionRange = versionRange;
            MaxVersion = new Version3x(VersionRange.MaxVersion);
            MinVersion = new Version3x(VersionRange.MinVersion);
        }

        public VersionRange VersionRange { get; }

        public bool HasLowerBound => VersionRange.HasLowerBound;
        public bool HasUpperBound => VersionRange.HasUpperBound;
        public bool IsFloating => VersionRange.IsFloating;
        public bool IsMaxInclusive => VersionRange.IsMaxInclusive;
        public bool IsMinInclusive => VersionRange.IsMinInclusive;
        public IVersion MaxVersion { get; }
        public IVersion MinVersion { get; }
        public string NormalizedString => VersionRange.ToNormalizedString();
        public string PrettyPrint => VersionRange.PrettyPrint();
        public string LegacyShortString => VersionRange.ToLegacyShortString();
        public string LegacyString => VersionRange.ToLegacyString();
        public string OriginalString => VersionRange.OriginalString;
        public bool LegacyShortStringAvailable => StaticLegacyShortStringAvailable;
        public bool IsFloatingAvailable => true;
        public bool OriginalStringAvailable => true;
        public bool LegacyStringAvailable => true;
    }
}
