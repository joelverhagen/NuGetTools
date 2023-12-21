namespace Knapcode.NuGetTools.Logic.Wrappers;

public interface IVersion
{
    int Revision { get; }
    bool IsSemVer2 { get; }
    bool IsPrerelease { get; }
    string NormalizedString { get; }
    bool NormalizedStringAvailable { get; }
    string FullString { get; }
    bool IsSemVer2Available { get; }
    bool FullStringAvailable { get; }
    string ToStringResult { get; }
}
