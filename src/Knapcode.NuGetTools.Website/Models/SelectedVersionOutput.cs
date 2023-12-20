namespace Knapcode.NuGetTools.Website
{
    public class SelectedVersionOutput
    {
        public required string CurrentVersion { get; set; }
        public required IEnumerable<VersionUrl> VersionUrls { get; set; }
    }
}
