using System;
using Knapcode.NuGetTools.Logic.Wrappers;
using NuGetVersionRange = NuGet.Versioning.VersionRange;

namespace Knapcode.NuGetTools.Logic.Direct.Wrappers
{
    public class VersionRange : IVersionRange
    {
        public VersionRange(NuGetVersionRange versionRange)
        {
            NuGetVersionRange = versionRange;
            MaxVersion = new Version(NuGetVersionRange.MaxVersion);
            MinVersion = new Version(NuGetVersionRange.MinVersion);
        }

        public NuGetVersionRange NuGetVersionRange { get; }
        public bool HasLowerBound => NuGetVersionRange.HasLowerBound;
        public bool HasUpperBound => NuGetVersionRange.HasUpperBound;
        public bool IsFloating => NuGetVersionRange.IsFloating;
        public bool IsMaxInclusive => NuGetVersionRange.IsMaxInclusive;
        public bool IsMinInclusive => NuGetVersionRange.IsMinInclusive;
        public IVersion MaxVersion { get; }
        public IVersion MinVersion { get; }
        public string NormalizedString => NuGetVersionRange.ToNormalizedString();
        public string PrettyPrint => NuGetVersionRange.PrettyPrint();
        public string LegacyShortString => NuGetVersionRange.ToLegacyShortString();
        public string LegacyString => NuGetVersionRange.ToLegacyString();
        public string OriginalString => NuGetVersionRange.OriginalString;
        public bool LegacyShortStringAvailable => true;
    }
}
