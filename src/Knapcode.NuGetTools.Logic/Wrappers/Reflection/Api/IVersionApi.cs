namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public interface IVersionApi
    {
        int Compare(object nuGetVersionA, object nuGetVersionB);
        string GetFullString(object nuGetVersion);
        bool GetFullStringAvailable();
        string GetNormalizedString(object nuGetVersion);
        bool GetNormalizedStringAvailable();
        string GetToStringResult(object nuGetVersion);
        int GetRevision(object nuGetVersion);
        bool IsPrerelease(object nuGetVersion);
        bool IsSemVer2(object nuGetVersion);
        bool IsSemVer2Available();
        object Parse(string input);
    }
}