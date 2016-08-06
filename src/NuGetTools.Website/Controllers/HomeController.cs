using System.Linq;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Microsoft.AspNetCore.Mvc;

namespace Knapcode.NuGetTools.Website
{
    public class HomeController : Controller
    {
        private readonly IToolsFactory _toolsFactory;

        public HomeController(IToolsFactory toolsFactory)
        {
            _toolsFactory = toolsFactory;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            var version = _toolsFactory
                .GetAvailableVersions()
                .First();

            return new RedirectToActionResult(nameof(SelectedVersionIndex), "Home", new { version })
            {
                Permanent = false
            };
        }

        private bool TryGetToolsService(string version, out IToolsService toolsService, out IActionResult notFoundResult)
        {
            toolsService = _toolsFactory.GetService(version);

            if (toolsService == null)
            {
                notFoundResult = new StatusCodeResult(404);
                return false;
            }

            notFoundResult = null;
            return true;
        }

        [HttpGet("/{version}")]
        public IActionResult SelectedVersionIndex(string version)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.Version;

            return View(output);
        }

        [HttpGet("/{version}/parse-framework")]
        public IActionResult ParseFramework(string version, ParseFrameworkInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseFramework(input);

            return View(output);
        }

        [HttpGet("/{version}/parse-version")]
        public IActionResult ParseVersion(string version, ParseVersionInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseVersion(input);

            return View(output);
        }

        [HttpGet("/{version}/parse-version-range")]
        public IActionResult ParseVersionRange(string version, ParseVersionRangeInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.ParseVersionRange(input);

            return View(output);
        }

        [HttpGet("/{version}/framework-compatibility")]
        public IActionResult FrameworkCompatibility(string version, FrameworkCompatibilityInput input, bool swap)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

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

            var output = toolsService.FrameworkCompatibility(input);
                        
            return View(output);
        }

        [HttpGet("/{version}/version-comparison")]
        public IActionResult VersionComparison(string version, VersionComparisonInput input, bool swap)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

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

            var output = toolsService.VersionComparison(input);

            return View(output);
        }

        [HttpGet("/{version}/get-nearest-framework")]
        public IActionResult GetNearestFramework(string version, GetNearestFrameworkInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.GetNearestFramework(input);

            return View(output);
        }

        [HttpGet("/{version}/version-satisfies")]
        public IActionResult VersionSatisfies(string version, VersionSatisfiesInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.VersionSatisfies(input);

            return View(output);
        }

        [HttpGet("/{version}/find-best-version-match")]
        public IActionResult FindBestVersionmatch(string version, FindBestVersionMatchInput input)
        {
            var toolsService = _toolsFactory.GetService(version);
            if (toolsService == null)
            {
                return NotFound();
            }

            var output = toolsService.FindBestVersionMatch(input);

            return View(output);
        }
    }
}