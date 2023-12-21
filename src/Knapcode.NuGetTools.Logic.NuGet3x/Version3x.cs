using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Logic.NuGet3x;

public class Version3x : IVersion
{
    private static readonly bool StaticFullStringAvailable = typeof(NuGetVersion)
        .GetMethod(nameof(NuGet.Versioning.NuGetVersion.ToFullString)) is not null;
    private static readonly bool StaticIsSemVer2Available = typeof(NuGetVersion)
        .GetMethod(nameof(NuGet.Versioning.NuGetVersion.IsSemVer2)) is not null;
    public Version3x(NuGetVersion version)
    {
        NuGetVersion = version;
    }

    public NuGetVersion NuGetVersion { get; }

    public string FullString => NuGetVersion.ToFullString();
    public bool IsPrerelease => NuGetVersion.IsPrerelease;
    public bool IsSemVer2 => NuGetVersion.IsSemVer2;
    public string NormalizedString => NuGetVersion.ToNormalizedString();
    public int Revision => NuGetVersion.Revision;
    public bool IsSemVer2Available => StaticIsSemVer2Available;
    public bool FullStringAvailable => StaticFullStringAvailable;
    public string ToStringResult => NuGetVersion.ToString();
    public bool NormalizedStringAvailable => true;
}
