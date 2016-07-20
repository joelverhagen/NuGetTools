using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NuGet.Frameworks;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class HomeController : Controller
    {
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
            var output = new ParseFrameworkOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Framework))
            {
                try
                {
                    output.Framework = NuGetFramework.Parse(input.Framework);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version")]
        public IActionResult ParseVersion(ParseVersionInput input)
        {
            var output = new ParseVersionOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Version))
            {
                try
                {
                    output.Version = NuGetVersion.Parse(input.Version);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/parse-version-range")]
        public IActionResult ParseVersionRange(ParseVersionRangeInput input)
        {
            var output = new ParseVersionRangeOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.VersionRange))
            {
                try
                {
                    output.VersionRange = VersionRange.Parse(input.VersionRange);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/framework-compatibility")]
        public IActionResult FrameworkCompatibility(FrameworkCompatibilityInput input)
        {
            if (input.Swap)
            {
                return new RedirectToActionResult(
                    nameof(FrameworkCompatibility),
                    "Home",
                    new { project = input.Package, package = input.Project })
                {
                    Permanent = false
                };
            }

            var output = new FrameworkCompatibilityOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };
            
            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Project) &&
                !string.IsNullOrWhiteSpace(input.Package))
            {
                try
                {
                    output.Project = NuGetFramework.Parse(input.Project);
                    output.IsProjectValid = true;
                }
                catch (Exception)
                {
                    output.IsProjectValid = false;
                }

                try
                {
                    output.Package = NuGetFramework.Parse(input.Package);
                    output.IsPackageValid = true;
                }
                catch (Exception)
                {
                    output.IsPackageValid = false;
                }

                if (output.IsProjectValid && output.IsPackageValid)
                {
                    output.InputStatus = InputStatus.Valid;
                    output.Compatible = DefaultCompatibilityProvider.Instance.IsCompatible(
                        output.Project,
                        output.Package);
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }
            
            return View(output);
        }

        [HttpGet("/3.5.0-beta2/version-comparison")]
        public IActionResult VersionComparison(VersionComparisonInput input)
        {
            if (input.Swap)
            {
                return new RedirectToActionResult(
                    nameof(VersionComparison),
                    "Home",
                    new { versionA = input.VersionB, versionB = input.VersionA })
                {
                    Permanent = false
                };
            }

            var output = new VersionComparisonOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.VersionA) &&
                !string.IsNullOrWhiteSpace(input.VersionB))
            {
                NuGetVersion versionA;
                output.IsVersionAValid = NuGetVersion.TryParse(input.VersionA, out versionA);
                output.VersionA = versionA;

                NuGetVersion versionB;
                output.IsVersionBValid = NuGetVersion.TryParse(input.VersionB, out versionB);
                output.VersionB = versionB;
;
                if (output.IsVersionAValid && output.IsVersionBValid)
                {
                    output.InputStatus = InputStatus.Valid;
                    var compare = output.VersionA.CompareTo(output.VersionB);
                    if (compare < 0)
                    {
                        output.Result = ComparisonResult.LessThan;
                    }
                    else if (compare == 0)
                    {
                        output.Result = ComparisonResult.Equal;
                    }
                    else
                    {
                        output.Result = ComparisonResult.GreaterThan;
                    }
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return View(output);
        }

        [HttpGet("/3.5.0-beta2/get-nearest-framework")]
        public IActionResult GetNearestFramework(GetNearestFrameworkInput input)
        {
            var package = new List<InputFramework>();
            var invalid = new List<string>();
            var output = new GetNearestFrameworkOutput
            {
                InputStatus = InputStatus.Missing,
                Package = package,
                Invalid = invalid,
                Input = input
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Project) &&
                !string.IsNullOrWhiteSpace(input.Package))
            {
                try
                {
                    output.Project = NuGetFramework.Parse(input.Project);
                    output.IsProjectValid = true;
                }
                catch (Exception)
                {
                    output.IsProjectValid = false;
                }
                
                using (var reader = new StringReader(input.Package))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        try
                        {
                            var framework = NuGetFramework.Parse(line);
                            var pair = new InputFramework
                            {
                                Input = line,
                                Framework = framework,
                                Compatible = DefaultCompatibilityProvider
                                    .Instance
                                    .IsCompatible(output.Project, framework)
                            };
                            package.Add(pair);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                output.IsPackageValid = output.Package.Any();
                
                if (output.IsProjectValid && output.IsPackageValid)
                {
                    output.InputStatus = InputStatus.Valid;

                    var reducer = new FrameworkReducer();
                    var frameworks = output.Package.Select(x => x.Framework).ToArray();
                    var nearest = reducer.GetNearest(output.Project, frameworks);
                    if (nearest != null)
                    {
                        output.Nearest = output.Package.First(x => x.Framework.Equals(nearest));
                    }
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return View(output);
        }
    }
}