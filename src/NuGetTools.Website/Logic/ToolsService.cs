using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NuGet.Frameworks;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website
{
    public class ToolsService : IToolsService
    {
        public ParseFrameworkOutput ParseFramework(ParseFrameworkInput input)
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

            return output;
        }

        public ParseVersionOutput ParseVersion(ParseVersionInput input)
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

            return output;
        }

        public ParseVersionRangeOutput ParseVersionRange(ParseVersionRangeInput input)
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

            return output;
        }

        public FrameworkCompatibilityOutput FrameworkCompatibility(FrameworkCompatibilityInput input)
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

            bool projectMissing = string.IsNullOrWhiteSpace(input.Project);
            bool packageMissing = string.IsNullOrWhiteSpace(input.Package);

            if (!projectMissing)
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
            }

            if (!packageMissing)
            {
                try
                {
                    output.Package = NuGetFramework.Parse(input.Package);
                    output.IsPackageValid = true;
                }
                catch (Exception)
                {
                    output.IsPackageValid = false;
                }
            }

            if (!projectMissing && !packageMissing)
            {
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

            return output;
        }

        public VersionComparisonOutput VersionComparison(VersionComparisonInput input)
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

            bool versionAMissing = string.IsNullOrWhiteSpace(input.VersionA);
            bool versionBMissing = string.IsNullOrWhiteSpace(input.VersionB);

            if (!versionAMissing)
            {
                try
                {
                    output.VersionA = NuGetVersion.Parse(input.VersionA);
                    output.IsVersionAValid = true;
                }
                catch
                {
                    output.IsVersionAValid = false;
                }
            }

            if (!versionBMissing)
            {
                try
                {
                    output.VersionB = NuGetVersion.Parse(input.VersionB);
                    output.IsVersionBValid = true;
                }
                catch
                {
                    output.IsVersionBValid = false;
                }
            }

            if (!versionAMissing && !versionBMissing)
            {
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

            return output;
        }

        public GetNearestFrameworkOutput GetNearestFramework(GetNearestFrameworkInput input)
        {
            var package = new List<OutputFramework>();
            var invalid = new List<string>();
            var output = new GetNearestFrameworkOutput
            {
                InputStatus = InputStatus.Missing,
                Package = package,
                Invalid = invalid,
                Input = input
            };

            if (input == null)
            {
                return output;
            }

            bool projectMissing = string.IsNullOrWhiteSpace(input.Project);
            bool packageMissing = string.IsNullOrWhiteSpace(input.Package);

            if (!projectMissing)
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
            }

            if (!packageMissing)
            {
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
                            var pair = new OutputFramework
                            {
                                Input = line,
                                Framework = framework,
                                Compatible = false
                            };

                            if (output.Project != null)
                            {
                                pair.Compatible = DefaultCompatibilityProvider
                                    .Instance
                                    .IsCompatible(output.Project, framework);
                            }

                            package.Add(pair);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                output.IsPackageValid = output.Package.Any();
            }

            if (!projectMissing && !packageMissing)
            {
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

            return output;
        }

        public VersionSatisfiesOutput VersionSatisfies(VersionSatisfiesInput input)
        {
            var output = new VersionSatisfiesOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            if (input == null)
            {
                return output;
            }

            bool versionRangeMissing = string.IsNullOrWhiteSpace(input.VersionRange);
            bool versionMissing = string.IsNullOrWhiteSpace(input.Version);

            if (!versionRangeMissing)
            {
                try
                {
                    output.VersionRange = VersionRange.Parse(input.VersionRange);
                    output.IsVersionRangeValid = true;
                }
                catch
                {
                    output.IsVersionRangeValid = false;
                }
            }

            if (!versionMissing)
            {
                try
                {
                    output.Version = NuGetVersion.Parse(input.Version);
                    output.IsVersionValid = true;
                }
                catch
                {
                    output.IsVersionValid = false;
                }
            }

            if (!versionRangeMissing && !versionMissing)
            {
                if (output.IsVersionRangeValid && output.IsVersionValid)
                {
                    output.InputStatus = InputStatus.Valid;
                    output.Satisfies = output.VersionRange.Satisfies(output.Version);
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }
    }
}
