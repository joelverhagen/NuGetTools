using System.Collections.Generic;

namespace Knapcode.NuGetTools.Website
{
    public class SelectedVersionOutput
    {
        public string CurrentVersion { get; set; }
        public IEnumerable<VersionUrl> VersionUrls { get; set; }
    }
}
