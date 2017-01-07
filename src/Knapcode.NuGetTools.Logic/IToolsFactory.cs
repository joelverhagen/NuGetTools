using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Knapcode.NuGetTools.Logic
{
    public interface IToolsFactory
    {
        Task<IEnumerable<string>> GetAvailableVersionsAsync(CancellationToken token);

        Task<IToolsService> GetServiceAsync(string version, CancellationToken token);

        Task<IFrameworkPrecedenceService> GetFrameworkPrecedenceServiceAsync(string version, CancellationToken token);
    }
}
