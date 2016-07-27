using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IVersionRangeLogic<TVersion, TVersionRange>
        where TVersion : IVersion
        where TVersionRange : IVersionRange
    {
        TVersionRange Parse(string input);
        bool Satisfies(TVersionRange versionRange, TVersion version);
        TVersion FindBestMatch(TVersionRange versionRange, IEnumerable<TVersion> versions);
    }
}
