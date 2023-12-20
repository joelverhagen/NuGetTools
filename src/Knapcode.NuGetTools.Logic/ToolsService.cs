using System.ComponentModel.Design;
using Knapcode.NuGetTools.Logic.Models;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class ToolsService : IToolsService
    {
        private readonly INuGetLogic _logic;

        public ToolsService(string version, INuGetLogic logic)
        {
            Version = version;
            _logic = logic;
        }

        public string Version { get; }

        public ParseFrameworkOutput ParseFramework(ParseFrameworkInput? input)
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
                    output.Framework = _logic.Framework.Parse(input.Framework);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public ParseVersionOutput ParseVersion(ParseVersionInput? input)
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
                    output.Version = _logic.Version.Parse(input.Version);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public ParseVersionRangeOutput ParseVersionRange(ParseVersionRangeInput? input)
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
                    output.VersionRange = _logic.VersionRange.Parse(input.VersionRange);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public FrameworkCompatibilityOutput FrameworkCompatibility(FrameworkCompatibilityInput? input)
        {
            var output = new FrameworkCompatibilityOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input == null)
            {
                return output;
            }

            bool projectMissing;
            if (!string.IsNullOrWhiteSpace(input.Project))
            {
                projectMissing = false;
                try
                {
                    output.Project = _logic.Framework.Parse(input.Project);
                    output.IsProjectValid = true;
                }
                catch (Exception)
                {
                    output.IsProjectValid = false;
                }
            }
            else
            {
                projectMissing = true;
            }

            bool packageMissing;
            if (!string.IsNullOrWhiteSpace(input.Package))
            {
                packageMissing = false;
                try
                {
                    output.Package = _logic.Framework.Parse(input.Package);
                    output.IsPackageValid = true;
                }
                catch (Exception)
                {
                    output.IsPackageValid = false;
                }
            }
            else
            {
                packageMissing = true;
            }

            if (!projectMissing && !packageMissing)
            {
                if (output.Project is not null && output.Package is not null)
                {
                    output.InputStatus = InputStatus.Valid;
                    output.IsCompatible = _logic.Framework.IsCompatible(output.Project, output.Package);
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public VersionComparisonOutput VersionComparison(VersionComparisonInput? input)
        {
            var output = new VersionComparisonOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input,
                Result = ComparisonResult.Equal
            };

            if (input == null)
            {
                return output;
            }

            bool missingVersionA;
            if (!string.IsNullOrWhiteSpace(input.VersionA))
            {
                missingVersionA = false;
                try
                {
                    output.VersionA = _logic.Version.Parse(input.VersionA);
                    output.IsVersionAValid = true;
                }
                catch
                {
                    output.IsVersionAValid = false;
                }
            }
            else
            {
                missingVersionA = true;
            }

            bool missingVersionB;
            if (!string.IsNullOrWhiteSpace(input.VersionB))
            {
                missingVersionB = false;
                try
                {
                    output.VersionB = _logic.Version.Parse(input.VersionB);
                    output.IsVersionBValid = true;
                }
                catch
                {
                    output.IsVersionBValid = false;
                }
            }
            else
            {
                missingVersionB = true;
            }

            if (!missingVersionA && !missingVersionB)
            {
                if (output.VersionA is not null && output.VersionB is not null)
                {
                    output.InputStatus = InputStatus.Valid;
                    var result = _logic.Version.Compare(output.VersionA, output.VersionB);
                    if (result < 0)
                    {
                        output.Result = ComparisonResult.LessThan;
                    }
                    else if (result == 0)
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

            return output;
        }

        public GetNearestFrameworkOutput GetNearestFramework(GetNearestFrameworkInput? input)
        {
            var outputFrameworks = new List<OutputFramework>();
            var invalid = new List<string>();
            var output = new GetNearestFrameworkOutput
            {
                InputStatus = InputStatus.Missing,
                Package = outputFrameworks,
                Invalid = invalid,
                Input = input
            };

            if (input == null)
            {
                return output;
            }

            bool projectMissing;
            if (!string.IsNullOrWhiteSpace(input.Project))
            {
                projectMissing = false;
                try
                {
                    output.Project = _logic.Framework.Parse(input.Project);
                    output.IsProjectValid = true;
                }
                catch (Exception)
                {
                    output.IsProjectValid = false;
                }
            }
            else
            {
                projectMissing = true;
            }

            bool packageMissing;
            if (!string.IsNullOrWhiteSpace(input.Package))
            {
                packageMissing = false;
                using (var reader = new StringReader(input.Package))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        try
                        {
                            var framework = _logic.Framework.Parse(line);
                            var pair = new OutputFramework
                            {
                                Input = line,
                                Framework = framework,
                                IsCompatible = false
                            };

                            if (output.Project is not null)
                            {
                                pair.IsCompatible = _logic.Framework.IsCompatible(output.Project, framework);
                            }

                            outputFrameworks.Add(pair);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                output.IsPackageValid = output.Package.Any();
            }
            else
            {
                packageMissing = true;
            }

            if (!projectMissing && !packageMissing)
            {
                if (output.Project is not null && output.IsPackageValid)
                {
                    output.InputStatus = InputStatus.Valid;

                    var frameworks = output.Package.Select(x => x.Framework).ToList();
                    var nearest = _logic.Framework.GetNearest(output.Project, frameworks);
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

            return output;
        }

        public VersionSatisfiesOutput VersionSatisfies(VersionSatisfiesInput? input)
        {
            var output = new VersionSatisfiesOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input,
            };

            if (input == null)
            {
                return output;
            }

            bool missingVersionRange;
            if (input.VersionRange is not null)
            {
                missingVersionRange = false;
                try
                {
                    output.VersionRange = _logic.VersionRange.Parse(input.VersionRange);
                    output.IsVersionRangeValid = true;
                }
                catch
                {
                    output.IsVersionRangeValid = false;
                }
            }
            else
            {
                missingVersionRange = true;
            }

            bool missingVersion;
            if (input.Version is not null)
            {
                missingVersion = false;
                try
                {
                    output.Version = _logic.Version.Parse(input.Version);
                    output.IsVersionValid = true;
                }
                catch
                {
                    output.IsVersionValid = false;
                }
            }
            else
            {
                missingVersion = true;
            }

            if (!missingVersionRange && !missingVersion)
            {
                if (output.VersionRange is not null && output.Version is not null)
                {
                    output.InputStatus = InputStatus.Valid;
                    output.Satisfies = _logic.VersionRange.Satisfies(output.VersionRange, output.Version);
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public FindBestVersionMatchOutput FindBestVersionMatch(FindBestVersionMatchInput? input)
        {
            var outputVersions = new List<OutputVersion>();
            var invalid = new List<string>();
            var output = new FindBestVersionMatchOutput
            {
                InputStatus = InputStatus.Missing,
                Versions = outputVersions,
                Invalid = invalid,
                Input = input,
                IsOperationSupported = _logic.VersionRange.FindBestMatchAvailable,
                AreVersionsSorted = _logic.VersionRange.IsBetterAvailable,
            };

            if (input == null)
            {
                return output;
            }

            bool versionRangeMissing;
            if (!string.IsNullOrWhiteSpace(input.VersionRange))
            {
                versionRangeMissing = false;
                try
                {
                    output.VersionRange = _logic.VersionRange.Parse(input.VersionRange);
                    output.IsVersionRangeValid = true;
                }
                catch (Exception)
                {
                    output.IsVersionRangeValid = false;
                }
            }
            else
            {
                versionRangeMissing = true;
            }

            bool versionsMissing;
            if (!string.IsNullOrWhiteSpace(input.Versions))
            {
                versionsMissing = false;
                using (var reader = new StringReader(input.Versions))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        try
                        {
                            var version = _logic.Version.Parse(line);
                            var pair = new OutputVersion
                            {
                                Input = line,
                                Version = version
                            };

                            if (output.VersionRange != null)
                            {
                                if (output.IsOperationSupported)
                                {
                                    var bestMatch = _logic.VersionRange.FindBestMatch(output.VersionRange, new[] { version });
                                    pair.Satisfies = bestMatch != null;
                                }
                                else
                                {
                                    pair.Satisfies = _logic.VersionRange.Satisfies(output.VersionRange, version);
                                }
                            }

                            outputVersions.Add(pair);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                if (output.VersionRange != null && _logic.VersionRange.IsBetterAvailable)
                {
                    outputVersions.Sort((a, b) => -1 * Compare(output.VersionRange, a, b));
                }

                output.IsVersionValid = output.Versions.Any();
            }
            else
            {
                versionsMissing = true;
            }

            if (!versionRangeMissing && !versionsMissing)
            {
                if (output.VersionRange is not null && output.IsVersionValid)
                {
                    output.InputStatus = InputStatus.Valid;

                    if (output.IsOperationSupported)
                    {
                        var versions = output.Versions.Select(x => x.Version).ToList();
                        var bestMatch = _logic.VersionRange.FindBestMatch(output.VersionRange, versions);
                        if (bestMatch != null)
                        {
                            output.BestMatch = output.Versions.First(x => x.Version.Equals(bestMatch));
                        }
                    }
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public SortVersionsOutput SortVersions(SortVersionsInput? input)
        {
            var outputVersions = new List<IVersion>();
            var invalid = new List<string>();
            var output = new SortVersionsOutput
            {
                InputStatus = InputStatus.Missing,
                Versions = outputVersions,
                Invalid = invalid,
                Input = input,
            };

            if (input == null)
            {
                return output;
            }

            bool versionsMissing;
            if (!string.IsNullOrWhiteSpace(input.Versions))
            {
                versionsMissing = false;
                using (var reader = new StringReader(input.Versions))
                {
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        line = line.Trim();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        try
                        {
                            var version = _logic.Version.Parse(line);
                            outputVersions.Add(version);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                outputVersions.Sort(_logic.Version.Compare);
            }
            else
            {
                versionsMissing = true;
            }

            if (!versionsMissing)
            {
                if (outputVersions.Any())
                {
                    output.InputStatus = InputStatus.Valid;
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        private int Compare(IVersionRange versionRange, OutputVersion a, OutputVersion b)
        {
            var satisfiesComparison = a.Satisfies.CompareTo(b.Satisfies);
            if (satisfiesComparison != 0)
            {
                return satisfiesComparison;
            }
            else if (_logic.VersionRange.IsBetter(versionRange, a.Version, b.Version))
            {
                return -1;
            }
            else if (_logic.VersionRange.IsBetter(versionRange, b.Version, a.Version))
            {
                return 1;
            }
            else
            {
                return _logic.Version.Compare(a.Version, b.Version);
            }
        }
    }
}
