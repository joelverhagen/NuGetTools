using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Microsoft.AspNetCore.Mvc;

namespace Knapcode.NuGetTools.Website
{
    public class HomeController : Controller
    {
        private readonly IToolsService _toolsService;

        public HomeController(IToolsService toolsService)
        {
            _toolsService = toolsService;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return new RedirectToActionResult(nameof(SelectedVersionIndex), "Home", null)
            {
                Permanent = false
            };
        }

        [HttpGet("/3.5.0-beta2")]
        public IActionResult SelectedVersionIndex()
        {
            return View();
        }

        [HttpGet("/3.5.0-beta2/parse-framework")]
        public IActionResult ParseFramework(ParseFrameworkInput input)
        {
            var output = _toolsService.ParseFramework(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version")]
        public IActionResult ParseVersion(ParseVersionInput input)
        {
            var output = _toolsService.ParseVersion(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version-range")]
        public IActionResult ParseVersionRange(ParseVersionRangeInput input)
        {
            var output = _toolsService.ParseVersionRange(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/framework-compatibility")]
        public IActionResult FrameworkCompatibility(FrameworkCompatibilityInput input, bool swap)
        {
            if (swap)
            {
                return new RedirectToActionResult(
                    nameof(FrameworkCompatibility),
                    "Home",
                    new { project = input.Package, package = input.Project })
                {
                    Permanent = false
                };
            }

            var output = _toolsService.FrameworkCompatibility(input);
                        
            return View(output);
        }

        [HttpGet("/3.5.0-beta2/version-comparison")]
        public IActionResult VersionComparison(VersionComparisonInput input, bool swap)
        {
            if (swap)
            {
                return new RedirectToActionResult(
                    nameof(VersionComparison),
                    "Home",
                    new { versionA = input.VersionB, versionB = input.VersionA })
                {
                    Permanent = false
                };
            }

            var output = _toolsService.VersionComparison(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/get-nearest-framework")]
        public IActionResult GetNearestFramework(GetNearestFrameworkInput input)
        {
            var output = _toolsService.GetNearestFramework(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/version-satisfies")]
        public IActionResult VersionSatisfies(VersionSatisfiesInput input)
        {
            var output = _toolsService.VersionSatisfies(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/find-best-version-match")]
        public IActionResult FindBestVersionmatch(FindBestVersionMatchInput input)
        {
            var output = _toolsService.FindBestVersionMatch(input);

            return View(output);
        }
    }
}