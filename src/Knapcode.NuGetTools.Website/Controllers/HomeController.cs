using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Knapcode.NuGetTools.Website;

public class HomeController : Controller
{
    /// <summary>
    /// This is the canonical first version that supports "*-*" for version ranges. The specification is here:
    /// https://github.com/NuGet/Home/pull/9104
    /// </summary>
    private const string StarDashStarVersion = "5.6.0";

    private static readonly Dictionary<string, string> VersionRedirects = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "3.5.0-beta2", "3.5.0-beta2-1484" },
        { "5.5.0-floating.7018", StarDashStarVersion },
        { "5.5.0-floating.7250", StarDashStarVersion },
        { "5.5.0-floating.7611", StarDashStarVersion }
    };

    private readonly IToolsFactory _toolsFactory;
    private readonly IUrlHelperFactory _urlHelperFactory;

    public HomeController(IToolsFactory toolsFactory, IUrlHelperFactory urlHelperFactory)
    {
        _toolsFactory = toolsFactory;
        _urlHelperFactory = urlHelperFactory;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken token)
    {
        var nuGetVersion = await _toolsFactory.GetLatestVersionAsync(token);

        return new RedirectToActionResult(
            nameof(SelectedVersionIndex),
            ControllerContext.ActionDescriptor.ControllerName,
            new { nuGetVersion },
            permanent: false);
    }

    [HttpGet("/api/versions")]
    public async Task<IActionResult> Versions(CancellationToken token)
    {
        var versions = await _toolsFactory.GetAvailableVersionsAsync(token);
        return new JsonResult(versions);
    }

    [Route("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return new ContentResult
        {
            Content = "An internal server error has occurred.",
            ContentType = "text/plain",
            StatusCode = 500
        };
    }

    [HttpGet("/{nuGetVersion}")]
    public async Task<IActionResult> SelectedVersionIndex([FromRoute] string nuGetVersion, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/parse-framework")]
    public async Task<IActionResult> ParseFramework([FromRoute] string nuGetVersion, [FromQuery] ParseFrameworkInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.ParseFramework(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/parse-version")]
    public async Task<IActionResult> ParseVersion([FromRoute] string nuGetVersion, [FromQuery] ParseVersionInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.ParseVersion(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/parse-version-range")]
    public async Task<IActionResult> ParseVersionRange([FromRoute] string nuGetVersion, [FromQuery] ParseVersionRangeInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.ParseVersionRange(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/framework-compatibility")]
    public async Task<IActionResult> FrameworkCompatibility([FromRoute] string nuGetVersion, [FromQuery] FrameworkCompatibilityInput input, bool swap, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        if (swap)
        {
            return new RedirectToActionResult(
                nameof(FrameworkCompatibility),
                ControllerContext.ActionDescriptor.ControllerName,
                new { project = input.Package, package = input.Project },
                permanent: false)
            {
                Permanent = false
            };
        }

        var output = toolsService.FrameworkCompatibility(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/version-comparison")]
    public async Task<IActionResult> VersionComparison([FromRoute] string nuGetVersion, [FromQuery] VersionComparisonInput input, bool swap, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        if (swap)
        {
            return new RedirectToActionResult(
                nameof(VersionComparison),
                ControllerContext.ActionDescriptor.ControllerName,
                new { versionA = input.VersionB, versionB = input.VersionA },
                permanent: false);
        }

        var output = toolsService.VersionComparison(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/sort-versions")]
    public async Task<IActionResult> SortVersions([FromRoute] string nuGetVersion, [FromQuery] SortVersionsInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.SortVersions(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/get-nearest-framework")]
    public async Task<IActionResult> GetNearestFramework([FromRoute] string nuGetVersion, [FromQuery] GetNearestFrameworkInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.GetNearestFramework(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/version-satisfies")]
    public async Task<IActionResult> VersionSatisfies([FromRoute] string nuGetVersion, [FromQuery] VersionSatisfiesInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.VersionSatisfies(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/find-best-version-match")]
    public async Task<IActionResult> FindBestVersionMatch([FromRoute] string nuGetVersion, [FromQuery] FindBestVersionMatchInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var toolsService = await _toolsFactory.GetServiceAsync(nuGetVersion, token);
        if (toolsService == null)
        {
            return NotFound();
        }

        var output = toolsService.FindBestVersionMatch(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/framework-precedence")]
    public async Task<IActionResult> FrameworkPrecedence([FromRoute] string nuGetVersion, [FromQuery] FrameworkPrecedenceInput input, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var service = await _toolsFactory.GetFrameworkPrecedenceServiceAsync(nuGetVersion, token);
        if (service == null)
        {
            return NotFound();
        }

        var output = service.FrameworkPrecedence(input);
        var versionedOutput = await GetSelectedVersionOutputAsync(service, output, token);

        return View(versionedOutput);
    }

    [HttpGet("/{nuGetVersion}/package-metadata")]
    public async Task<IActionResult> PackageMetadata([FromRoute] string nuGetVersion, CancellationToken token)
    {
        var redirect = await GetVersionRedirectAsync(token);
        if (redirect != null)
        {
            return redirect;
        }

        var packages = await _toolsFactory.GetPackagesAsync(nuGetVersion, token);
        if (packages == null)
        {
            return NotFound();
        }

        var output = await GetSelectedVersionOutputAsync(nuGetVersion, packages, token);

        return View(output);
    }

    private async Task<SelectedVersionOutput> GetSelectedVersionOutputAsync(IVersionedService service, CancellationToken token)
    {
        return await GetSelectedVersionOutputAsync(service.Version, (object?)null, token);
    }

    private async Task<SelectedVersionOutput<T>> GetSelectedVersionOutputAsync<T>(IVersionedService service, T value, CancellationToken token)
    {
        return await GetSelectedVersionOutputAsync(service.Version, value, token);
    }

    private async Task<SelectedVersionOutput<T>> GetSelectedVersionOutputAsync<T>(
        string currentVersion,
        T value,
        CancellationToken token)
    {
        var availableVersions = await _toolsFactory.GetAvailableVersionsAsync(token);
        var versionUrls = GetVersionUrls(availableVersions);

        return new SelectedVersionOutput<T>
        {
            CurrentVersion = currentVersion,
            VersionUrls = versionUrls,
            Output = value
        };
    }

    private IEnumerable<VersionUrl> GetVersionUrls(IEnumerable<string> nuGetVersions)
    {
        var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);

        foreach (var nuGetVersion in nuGetVersions)
        {
            var url = GetVersionUrl(nuGetVersion, urlHelper);

            yield return new VersionUrl
            {
                Version = nuGetVersion,
                Url = url
            };
        }
    }

    private async Task<IActionResult?> GetVersionRedirectAsync(CancellationToken token)
    {
        // Get the current version.
        var nuGetVersion = (string?)ControllerContext.RouteData.Values["nuGetVersion"];

        // Determine if the current version needs a redirect.
        string? redirectNuGetVersion = null;
        if (nuGetVersion is not null && VersionRedirects.TryGetValue(nuGetVersion, out var knownRedirect))
        {
            redirectNuGetVersion = knownRedirect;
        }
        else if (nuGetVersion == "latest")
        {
            redirectNuGetVersion = await _toolsFactory.GetLatestVersionAsync(token);
        }

        if (redirectNuGetVersion != null)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
            var redirect = GetVersionUrl(redirectNuGetVersion, urlHelper);

            return new RedirectResult(redirect, permanent: false);
        }

        return null;
    }

    private string GetVersionUrl(string nuGetVersion, IUrlHelper urlHelper)
    {
        var url = urlHelper.Action(
            ControllerContext.ActionDescriptor.ActionName,
            ControllerContext.ActionDescriptor.ControllerName,
            new { nuGetVersion });

        if (url is null)
        {
            throw new InvalidOperationException("Could not construct a URL.");
        }

        if (Request.QueryString.HasValue)
        {
            url += Request.QueryString.Value;
        }

        return url;
    }
}
