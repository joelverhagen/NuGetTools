using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public interface IVersionRangeApi
    {
        object FindBestMatch(object versionRange, IEnumerable<object> versions);
        bool FindBestMatchAvailable();
        string GetLegacyShortString(object nuGetVersionRange);
        bool GetLegacyShortStringAvailable();
        string GetLegacyString(object nuGetVersionRange);
        bool GetLegacyStringAvailable();
        object GetMaxVersion(object nuGetVersionRange);
        object GetMinVersion(object nuGetVersionRange);
        string GetNormalizedString(object nuGetVersionRange);
        string GetOriginalString(object nuGetVersionRange);
        bool GetOriginalStringAvailable();
        bool HasLowerBound(object nuGetVersionRange);
        bool HasUpperBound(object nuGetVersionRange);
        bool IsFloating(object nuGetVersionRange);
        bool IsFloatingAvailable();
        bool IsMaxInclusive(object nuGetVersionRange);
        bool IsMinInclusive(object nuGetVersionRange);
        object Parse(string input);
        string PrettyPrint(object nuGetVersionRange);
        bool Satisfies(object nuGetVersionRange, object nuGetVersion);
    }
}