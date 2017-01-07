using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Knapcode.NuGetTools.Website
{
    public class HomeController : Controller
    {
        private static readonly Dictionary<string, string> _versionRedirects = new Dictionary<string, string>
        {
            { "3.5.0-beta2", "3.5.0-beta2-1484" }
        };

        private static readonly string _controllerName;

        static HomeController()
        {
            var typeName = typeof(HomeController).Name;
            ControllerName = typeName.Substring(0, typeName.Length - "Controller".Length);
        }

        private readonly IToolsFactory _toolsFactory;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public HomeController(IToolsFactory toolsFactory, IUrlHelperFactory urlHelperFactory)
        {
            _toolsFactory = toolsFactory;
            _urlHelperFactory = urlHelperFactory;
        }

        public static string ControllerName { get; }

        [HttpGet("/")]
        public async Task<IActionResult> Index(CancellationToken token)
        {
            var versions = await _toolsFactory.GetAvailableVersionsAsync(token);

            var version = versions.First();

            return new RedirectToActionResult(
                nameof(SelectedVersionIndex),
                ControllerContext.ActionDescriptor.ControllerName,
                new { version },
                permanent: false);
        }

        [HttpGet("/{version}")]
        public async Task<IActionResult> SelectedVersionIndex([FromRoute]string version, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }
            
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/parse-framework")]
        public async Task<IActionResult> ParseFramework([FromRoute]string version, [FromQuery]ParseFrameworkInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseFramework(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/parse-version")]
        public async Task<IActionResult> ParseVersion([FromRoute]string version, [FromQuery]ParseVersionInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseVersion(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/parse-version-range")]
        public async Task<IActionResult> ParseVersionRange([FromRoute]string version, [FromQuery]ParseVersionRangeInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseVersionRange(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/framework-compatibility")]
        public async Task<IActionResult> FrameworkCompatibility([FromRoute]string version, [FromQuery]FrameworkCompatibilityInput input, bool swap, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
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

        [HttpGet("/{version}/version-comparison")]
        public async Task<IActionResult> VersionComparison([FromRoute]string version, [FromQuery]VersionComparisonInput input, bool swap, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
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

        [HttpGet("/{version}/get-nearest-framework")]
        public async Task<IActionResult> GetNearestFramework([FromRoute]string version, [FromQuery]GetNearestFrameworkInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.GetNearestFramework(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/version-satisfies")]
        public async Task<IActionResult> VersionSatisfies([FromRoute]string version, [FromQuery]VersionSatisfiesInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.VersionSatisfies(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/find-best-version-match")]
        public async Task<IActionResult> FindBestVersionMatch([FromRoute]string version, [FromQuery]FindBestVersionMatchInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var toolsService = await _toolsFactory.GetServiceAsync(version, token);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.FindBestVersionMatch(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(toolsService, output, token);

            return View(versionedOutput);
        }

        [HttpGet("/{version}/framework-precedence")]
        public async Task<IActionResult> FrameworkPrecedence([FromRoute]string version, [FromQuery]FrameworkPrecedenceInput input, CancellationToken token)
        {
            var redirect = GetVersionRedirect();
            if (redirect != null)
            {
                return redirect;
            }

            var service = await _toolsFactory.GetFrameworkPrecedenceServiceAsync(version, token);
            if (service == null)
            {
                return NotFound();
            }

            var output = service.FrameworkPrecedence(input);
            var versionedOutput = await GetSelectedVersionOutputAsync(service, output, token);

            return View(versionedOutput);
        }

        private async Task<SelectedVersionOutput> GetSelectedVersionOutputAsync(IVersionedService service, CancellationToken token)
        {
            return await GetSelectedVersionOutputAsync(service.Version, (object)null, token);
        }

        private async Task<SelectedVersionOutput<T>> GetSelectedVersionOutputAsync<T>(IVersionedService service, T value, CancellationToken token)
        {
            return await GetSelectedVersionOutputAsync(service.Version, value, token);
        }

        private async Task<SelectedVersionOutput<T>> GetSelectedVersionOutputAsync<T>(string currentVersion, T value, CancellationToken token)
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

        private IEnumerable<VersionUrl> GetVersionUrls(IEnumerable<string> versions)
        {
            var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);

            foreach (var version in versions)
            {
                var url = GetVersionUrl(version, urlHelper);

                yield return new VersionUrl
                {
                    Version = version,
                    Url = url
                };
            }
        }

        private IActionResult GetVersionRedirect()
        {
            // Get the current version.
            var version = (string) ControllerContext.RouteData.Values["version"];
             
            // Determine if the current version needs a redirect.
            string redirectVersion;
            if (_versionRedirects.TryGetValue(version, out redirectVersion))
            {
                var urlHelper = _urlHelperFactory.GetUrlHelper(ControllerContext);
                var redirect = GetVersionUrl(redirectVersion, urlHelper);

                return new RedirectResult(redirect, permanent: false);
            }

            return null;
        }

        private string GetVersionUrl(string version, IUrlHelper urlHelper)
        {
            var url = urlHelper.Action(
                ControllerContext.ActionDescriptor.ActionName,
                ControllerContext.ActionDescriptor.ControllerName,
                new { version });

            if (Request.QueryString.HasValue)
            {
                url += Request.QueryString.Value;
            }

            return url;
        }
    }
}