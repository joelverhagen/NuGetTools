using Microsoft.AspNetCore.Mvc;

namespace Knapcode.NuGetTools.Website
{
    public class HomeController : Controller
    {
        private readonly IToolsService _toolService;

        public HomeController()
        {
            _toolService = new ToolsService();
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
            var output = _toolService.ParseFramework(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version")]
        public IActionResult ParseVersion(ParseVersionInput input)
        {
            var output = _toolService.ParseVersion(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version-range")]
        public IActionResult ParseVersionRange(ParseVersionRangeInput input)
        {
            var output = _toolService.ParseVersionRange(input);

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

            var output = _toolService.FrameworkCompatibility(input);
                        
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

            var output = _toolService.VersionComparison(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/get-nearest-framework")]
        public IActionResult GetNearestFramework(GetNearestFrameworkInput input)
        {
            var output = _toolService.GetNearestFramework(input);

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/version-satisfies")]
        public IActionResult VersionSatisfies(VersionSatisfiesInput input)
        {
            var output = _toolService.VersionSatisfies(input);

            return View(output);
        }
    }
}