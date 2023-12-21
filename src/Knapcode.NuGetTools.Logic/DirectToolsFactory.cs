namespace Knapcode.NuGetTools.Logic;

public class DirectToolsFactory : IToolsFactory
{
    private readonly string _version;
    private readonly IToolsService _toolsService;
    private readonly IFrameworkPrecedenceService _frameworkPrecedenceService;
    private readonly IFrameworkList _frameworkList;

    public DirectToolsFactory(
        IToolsService toolsService,
        IFrameworkPrecedenceService frameworkPrecedenceService,
        IFrameworkList frameworkList)
    {
        _version = toolsService.Version;
        _toolsService = toolsService;
        _frameworkPrecedenceService = frameworkPrecedenceService;
        _frameworkList = frameworkList;
    }

    public IEnumerable<string> GetAvailableVersions()
    {
        return new[] { _version };
    }

    public IToolsService? GetService(string version)
    {
        if (version != _version)
        {
            return null;
        }

        return _toolsService;
    }

    public Task<IEnumerable<string>> GetAvailableVersionsAsync(CancellationToken token)
    {
        var versions = new[] { _version };

        return Task.FromResult<IEnumerable<string>>(versions);
    }

    public Task<IToolsService?> GetServiceAsync(string version, CancellationToken token)
    {
        IToolsService? output = null;

        if (version == _version)
        {
            output = _toolsService;
        }

        return Task.FromResult(output);
    }

    public Task<IFrameworkPrecedenceService?> GetFrameworkPrecedenceServiceAsync(string version, CancellationToken token)
    {
        IFrameworkPrecedenceService? output = null;

        if (version == _version)
        {
            output = _frameworkPrecedenceService;
        }

        return Task.FromResult(output);
    }

    public Task<IFrameworkList> GetFrameworkListAsync(CancellationToken token)
    {
        return Task.FromResult(_frameworkList);
    }

    public Task<string> GetLatestVersionAsync(CancellationToken token)
    {
        return Task.FromResult(_version);
    }
}
