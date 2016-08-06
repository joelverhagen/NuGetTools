using Knapcode.NuGetTools.Logic.Models;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;

namespace Knapcode.NuGetTools.Logic
{
    public interface IToolsService
    {
        FrameworkCompatibilityOutput FrameworkCompatibility(FrameworkCompatibilityInput input);
        GetNearestFrameworkOutput GetNearestFramework(GetNearestFrameworkInput input);
        ParseFrameworkOutput ParseFramework(ParseFrameworkInput input);
        ParseVersionOutput ParseVersion(ParseVersionInput input);
        ParseVersionRangeOutput ParseVersionRange(ParseVersionRangeInput input);
        VersionComparisonOutput VersionComparison(VersionComparisonInput input);
        VersionSatisfiesOutput VersionSatisfies(VersionSatisfiesInput input);
        FindBestVersionMatchOutput FindBestVersionMatch(FindBestVersionMatchInput input);
        VersionOutput Version { get; }
    }
}