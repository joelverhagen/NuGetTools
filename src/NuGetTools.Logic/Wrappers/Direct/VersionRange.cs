using NuGetVersionRange = NuGet.Versioning.VersionRange;

namespace Knapcode.NuGetTools.Logic.Wrappers.Direct
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
    }
}
