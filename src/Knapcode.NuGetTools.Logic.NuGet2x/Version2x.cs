using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x
{
    public class Version2x : IVersion
    {
        private static readonly bool StaticNormalizedStringAvailable = typeof(SemanticVersion)
            .GetMethod(nameof(SemanticVersion.ToNormalizedString)) is not null;
        private static readonly bool StaticFullStringAvailable = typeof(SemanticVersion)
            .GetMethod(nameof(SemanticVersion.ToFullString)) is not null;
        private static readonly bool StaticIsSemVer2Available = typeof(SemanticVersion)
            .GetMethod(nameof(SemanticVersion.IsSemVer2)) is not null;

        public Version2x(SemanticVersion version)
        {
            SemanticVersion = version;
        }

        public SemanticVersion SemanticVersion { get; }

        public int Revision => SemanticVersion.Version.Revision;
        public bool IsSemVer2 => SemanticVersion.IsSemVer2();
        public bool IsPrerelease => !string.IsNullOrEmpty(SemanticVersion.SpecialVersion);
        public string NormalizedString => SemanticVersion.ToNormalizedString();
        public bool NormalizedStringAvailable => StaticNormalizedStringAvailable;
        public string FullString => SemanticVersion.ToFullString();
        public bool IsSemVer2Available => StaticIsSemVer2Available;
        public bool FullStringAvailable => StaticFullStringAvailable;
        public string ToStringResult => SemanticVersion.ToString();
    }
}
