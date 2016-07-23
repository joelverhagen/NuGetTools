namespace Knapcode.NuGetTools.Website
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
    }
}