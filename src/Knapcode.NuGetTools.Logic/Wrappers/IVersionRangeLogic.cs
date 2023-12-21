namespace Knapcode.NuGetTools.Logic.Wrappers;

public interface IVersionRangeLogic
{
    IVersionRange Parse(string input);
    bool Satisfies(IVersionRange versionRange, IVersion version);
    IVersion? FindBestMatch(IVersionRange versionRange, IEnumerable<IVersion> versions);
    bool IsBetter(IVersionRange versionRange, IVersion current, IVersion considering);
    bool FindBestMatchAvailable { get; }
    bool IsBetterAvailable { get; }
}
