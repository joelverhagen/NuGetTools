namespace Knapcode.NuGetTools.Logic.Models.Framework;

public class FrameworkPrecedenceInput
{
    public bool IncludeProfiles { get; set; }
    public bool ExcludePortable { get; set; }
    public string? ExcludedIdentifiers { get; set; }
    public string? Framework { get; set; }
}