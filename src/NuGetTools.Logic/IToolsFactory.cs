using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic
{
    public interface IToolsFactory
    {
        IEnumerable<string> GetAvailableVersions();

        IToolsService GetService(string version);
    }
}
