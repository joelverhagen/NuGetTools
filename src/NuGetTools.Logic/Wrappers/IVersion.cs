namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IVersion
    {
        int Revision { get; }
        bool IsSemVer2 { get; }
        bool IsPrerelease { get; }
        string NormalizedString { get; }
        string FullString { get; }
    }
}
