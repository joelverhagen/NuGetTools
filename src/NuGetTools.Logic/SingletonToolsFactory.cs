using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic
{
    public class SingletonToolsFactory : IToolsFactory
    {
        private readonly string _version;
        private readonly IToolsService _toolsService;

        public SingletonToolsFactory(IToolsService toolsService)
        {
            _version = toolsService.Version.Version;
            _toolsService = toolsService;
        }

        public IEnumerable<string> GetAvailableVersions()
        {
            return new[] { _version };
        }

        public IToolsService GetService(string version)
        {
            if (version != _version)
            {
                return null;
            }

            return _toolsService;
        }
    }
}
