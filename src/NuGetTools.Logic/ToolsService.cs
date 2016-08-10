using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Knapcode.NuGetTools.Logic.Models;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Models.Version;
using Knapcode.NuGetTools.Logic.Models.VersionRange;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class ToolsService<TFramework, TVersion, TVersionRange> : IToolsService
        where TFramework : IFramework
        where TVersion : IVersion
        where TVersionRange : IVersionRange
    {
        private readonly string _version;
        private readonly IFrameworkLogic<TFramework> _frameworkLogic;
        private readonly IVersionLogic<TVersion> _versionLogic;
        private readonly IVersionRangeLogic<TVersion, TVersionRange> _versionRangeLogic;
        
        public ToolsService(
            string version,
            IFrameworkLogic<TFramework> frameworkLogic,
            IVersionLogic<TVersion> versionLogic,
            IVersionRangeLogic<TVersion, TVersionRange> versionRangeLogic)
        {
            _version = version;
            _frameworkLogic = frameworkLogic;
            _versionLogic = versionLogic;
            _versionRangeLogic = versionRangeLogic;
        }

        public SelectedVersionOutput Version
        {
            get
            {
                return new SelectedVersionOutput
                {
                    Version = _version
                };
            }
        }

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
                    output.Framework = _frameworkLogic.Parse(input.Framework);
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
                    output.Version = _versionLogic.Parse(input.Version);
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
                    output.VersionRange = _versionRangeLogic.Parse(input.VersionRange);
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

            var projectMissing = string.IsNullOrWhiteSpace(input.Project);
            var packageMissing = string.IsNullOrWhiteSpace(input.Package);

            var project = default(TFramework);
            if (!projectMissing)
            {
                try
                {
                    project = _frameworkLogic.Parse(input.Project);
                    output.Project = project;
                    output.IsProjectValid = true;
                }
                catch (Exception)
                {
                    output.IsProjectValid = false;
                }
            }

            var package = default(TFramework);
            if (!packageMissing)
            {
                try
                {
                    package = _frameworkLogic.Parse(input.Package);
                    output.Package = package;
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
                    output.IsCompatible = _frameworkLogic.IsCompatible(project, package);
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

            var versionAMissing = string.IsNullOrWhiteSpace(input.VersionA);
            var versionBMissing = string.IsNullOrWhiteSpace(input.VersionB);

            var versionA = default(TVersion);
            if (!versionAMissing)
            {
                try
                {
                    versionA = _versionLogic.Parse(input.VersionA);
                    output.VersionA = versionA;
                    output.IsVersionAValid = true;
                }
                catch
                {
                    output.IsVersionAValid = false;
                }
            }

            var versionB = default(TVersion);
            if (!versionBMissing)
            {
                try
                {
                    versionB = _versionLogic.Parse(input.VersionB);
                    output.VersionB = versionB;
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
                    var result = _versionLogic.Compare(versionA, versionB);
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

        public GetNearestFrameworkOutput GetNearestFramework(GetNearestFrameworkInput input)
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

            var projectMissing = string.IsNullOrWhiteSpace(input.Project);
            var packageMissing = string.IsNullOrWhiteSpace(input.Package);

            var project = default(TFramework);
            if (!projectMissing)
            {
                try
                {
                    project = _frameworkLogic.Parse(input.Project);
                    output.Project = project;
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
                            var framework = _frameworkLogic.Parse(line);
                            var pair = new OutputFramework
                            {
                                Input = line,
                                Framework = framework,
                                IsCompatible = false
                            };

                            if (output.Project != null)
                            {
                                pair.IsCompatible = _frameworkLogic.IsCompatible(project, framework);
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

            if (!projectMissing && !packageMissing)
            {
                if (output.IsProjectValid && output.IsPackageValid)
                {
                    output.InputStatus = InputStatus.Valid;
                    
                    var frameworks = output.Package.Select(x => (TFramework)x.Framework).ToArray();
                    var nearest = _frameworkLogic.GetNearest(project, frameworks);
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

            var versionRangeMissing = string.IsNullOrWhiteSpace(input.VersionRange);
            var versionMissing = string.IsNullOrWhiteSpace(input.Version);

            var versionRange = default(TVersionRange);
            if (!versionRangeMissing)
            {
                try
                {
                    versionRange = _versionRangeLogic.Parse(input.VersionRange);
                    output.VersionRange = versionRange;
                    output.IsVersionRangeValid = true;
                }
                catch
                {
                    output.IsVersionRangeValid = false;
                }
            }

            var version = default(TVersion);
            if (!versionMissing)
            {
                try
                {
                    version =_versionLogic.Parse(input.Version);
                    output.Version = version;
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
                    output.Satisfies = _versionRangeLogic.Satisfies(versionRange, version);
                }
                else
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            return output;
        }

        public FindBestVersionMatchOutput FindBestVersionMatch(FindBestVersionMatchInput input)
        {
            var outputVersions = new List<OutputVersion>();
            var invalid = new List<string>();
            var output = new FindBestVersionMatchOutput
            {
                InputStatus = InputStatus.Missing,
                Versions = outputVersions,
                Invalid = invalid,
                Input = input
            };

            if (input == null)
            {
                return output;
            }

            var versionRangeMissing = string.IsNullOrWhiteSpace(input.VersionRange);
            var versionsMissing = string.IsNullOrWhiteSpace(input.Versions);

            var versionRange = default(TVersionRange);
            if (!versionRangeMissing)
            {
                try
                {
                    versionRange = _versionRangeLogic.Parse(input.VersionRange);
                    output.VersionRange = versionRange;
                    output.IsVersionRangeValid = true;
                }
                catch (Exception)
                {
                    output.IsVersionRangeValid = false;
                }
            }

            if (!versionsMissing)
            {
                using (var reader = new StringReader(input.Versions))
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
                            var version = _versionLogic.Parse(line);
                            var pair = new OutputVersion
                            {
                                Input = line,
                                Version = version
                            };

                            if (output.VersionRange != null)
                            {
                                pair.Satisfies = _versionRangeLogic.Satisfies(versionRange, version);
                            }

                            outputVersions.Add(pair);
                        }
                        catch (Exception)
                        {
                            invalid.Add(line);
                        }
                    }
                }

                output.IsVersionValid = output.Versions.Any();
            }

            if (!versionRangeMissing && !versionsMissing)
            {
                if (output.IsVersionRangeValid && output.IsVersionValid)
                {
                    output.InputStatus = InputStatus.Valid;
                    
                    var versions = output.Versions.Select(x => (TVersion)x.Version).ToArray();
                    var bestMatch = _versionRangeLogic.FindBestMatch(versionRange, versions);
                    if (bestMatch != null)
                    {
                        output.BestMatch = output.Versions.First(x => x.Version.Equals(bestMatch));
                    }
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
