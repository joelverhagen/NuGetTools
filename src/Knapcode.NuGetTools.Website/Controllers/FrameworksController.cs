using Knapcode.NuGetTools.Logic;
using Microsoft.AspNetCore.Mvc;

namespace Knapcode.NuGetTools.Website;

[Route("api/[controller]")]
public class FrameworksController : Controller
{
    private readonly IToolsFactory _toolsFactory;

    public FrameworksController(IToolsFactory toolsService)
    {
        _toolsFactory = toolsService;
    }

    [HttpGet("short-folder-names")]
    public async Task<IEnumerable<string>> GetShortFolderNames(CancellationToken token)
    {
        var frameworkList = await _toolsFactory.GetFrameworkListAsync(token);
        return frameworkList.ShortFolderNames;
    }

    [HttpGet("identifiers")]
    public async Task<IEnumerable<string>> GetIdentifiers(CancellationToken token)
    {
        var frameworkList = await _toolsFactory.GetFrameworkListAsync(token);
        return frameworkList.Identifiers;
    }
}
