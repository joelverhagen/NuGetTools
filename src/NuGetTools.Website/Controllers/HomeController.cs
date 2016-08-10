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

            var output = toolsService.Version;

            return View(output);
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

            return View(output);
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

            return View(output);
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

            return View(output);
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
                        
            return View(output);
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

            return View(output);
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

            return View(output);
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

            return View(output);
        }

        [HttpGet("/{version}/find-best-version-match")]
        public async Task<IActionResult> FindBestVersionmatch([FromRoute]string version, [FromQuery]FindBestVersionMatchInput input, CancellationToken token)
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

            return View(output);
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

                var redirect = urlHelper.Action(
                    ControllerContext.ActionDescriptor.ActionName,
                    ControllerContext.ActionDescriptor.ControllerName,
                    new { version = redirectVersion });

                if (Request.QueryString.HasValue)
                {
                    redirect += Request.QueryString.Value;
                }

                return new RedirectResult(redirect, permanent: false);
            }

            return null;
        }
    }
}