using System.Linq;
using NuGet.Frameworks;
using NuGet.Versioning;
using Xunit;

namespace Knapcode.NuGetTools.Website.Tests
{
    public class ToolsServiceTests
    {
        [Fact]
        public void ParseVersionRange_ValidVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionRangeInput
            {
                VersionRange = "(, 1.0.0-beta]"
            };

            // Act
            var output = target.ParseVersionRange(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
        }

        [Fact]
        public void ParseVersionRange_InvalidVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionRangeInput
            {
                VersionRange = "a"
            };

            // Act
            var output = target.ParseVersionRange(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.Null(output.VersionRange);
        }

        [Fact]
        public void ParseVersionRange_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            ParseVersionRangeInput input = null;

            // Act
            var output = target.ParseVersionRange(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.VersionRange);
        }

        [Fact]
        public void ParseVersionRange_MissingVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionRangeInput
            {
                VersionRange = null
            };

            // Act
            var output = target.ParseVersionRange(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.VersionRange);
        }

        [Fact]
        public void ParseVersion_ValidVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionInput
            {
                Version = "1.0.0-beta"
            };

            // Act
            var output = target.ParseVersion(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(NuGetVersion.Parse(input.Version), output.Version);
        }

        [Fact]
        public void ParseVersion_InvalidVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionInput
            {
                Version = "1.0.0.0.0"
            };

            // Act
            var output = target.ParseVersion(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.Null(output.Version);
        }

        [Fact]
        public void ParseVersion_MissingVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseVersionInput
            {
                Version = null
            };

            // Act
            var output = target.ParseVersion(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.Version);
        }

        [Fact]
        public void ParseVersion_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            ParseVersionInput input = null;

            // Act
            var output = target.ParseVersion(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.Version);
        }

        [Fact]
        public void ParseFramework_ValidFramework()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseFrameworkInput
            {
                Framework = ".NETFramework,Version=v4.5"
            };

            // Act
            var output = target.ParseFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(NuGetFramework.Parse(input.Framework), output.Framework);
        }

        [Fact]
        public void ParseFramework_InvalidFramework()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseFrameworkInput
            {
                Framework = ".NETPortable,Version=v4.5,Profile=net45+net-cf"
            };

            // Act
            var output = target.ParseFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.Null(output.Framework);
        }

        [Fact]
        public void ParseFramework_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            ParseFrameworkInput input = null;

            // Act
            var output = target.ParseFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.Framework);
        }

        [Fact]
        public void ParseFramework_MissingFramework()
        {
            // Arrange
            var target = new ToolsService();
            var input = new ParseFrameworkInput
            {
                Framework = null
            };

            // Act
            var output = target.ParseFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Null(output.Framework);
        }

        [Fact]
        public void FrameworkCompatibility_InvalidProject()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "portable-net45+net-cf",
                Package = "netstandard1.0"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Equal(NuGetFramework.Parse(input.Package), output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_InvalidPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "net45",
                Package = "portable-net45+net-cf"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.True(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Null(output.Package);            
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_InvalidProjectAndPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "portable-win8+net-cf",
                Package = "portable-net45+net-cf"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Null(output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_Compatible()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "net45",
                Package = "netstandard1.0"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Equal(NuGetFramework.Parse(input.Package), output.Package);
            Assert.True(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_Incompatible()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "net45",
                Package = "netstandard1.3"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Equal(NuGetFramework.Parse(input.Package), output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_MissingProject()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = null,
                Package = "netstandard1.3"
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Equal(NuGetFramework.Parse(input.Package), output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_MissingPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = "net45",
                Package = null
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.True(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Null(output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_MissingProjectAndPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FrameworkCompatibilityInput
            {
                Project = null,
                Package = null
            };

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Null(output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void FrameworkCompatibility_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            FrameworkCompatibilityInput input = null;

            // Act
            var output = target.FrameworkCompatibility(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Null(output.Package);
            Assert.False(output.Compatible);
        }

        [Fact]
        public void VersionComparison_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            VersionComparisonInput input = null;

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionAValid);
            Assert.False(output.IsVersionBValid);
            Assert.Null(output.VersionA);
            Assert.Null(output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_MissingVersionA()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = null,
                VersionB = "2.0"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionAValid);
            Assert.True(output.IsVersionBValid);
            Assert.Null(output.VersionA);
            Assert.Equal(NuGetVersion.Parse(input.VersionB), output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_MissingVersionB()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "1.0.0-beta",
                VersionB = null
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.True(output.IsVersionAValid);
            Assert.False(output.IsVersionBValid);
            Assert.Equal(NuGetVersion.Parse(input.VersionA), output.VersionA);
            Assert.Null(output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_MissingVersionAAndVersionB()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = null,
                VersionB = null
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionAValid);
            Assert.False(output.IsVersionBValid);
            Assert.Null(output.VersionA);
            Assert.Null(output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_LessThan()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "1.0",
                VersionB = "2.0.0-beta"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsVersionAValid);
            Assert.True(output.IsVersionBValid);
            Assert.Equal(NuGetVersion.Parse(input.VersionA), output.VersionA);
            Assert.Equal(NuGetVersion.Parse(input.VersionB), output.VersionB);
            Assert.Equal(ComparisonResult.LessThan, output.Result);
        }

        [Fact]
        public void VersionComparison_GreaterThan()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "2.0",
                VersionB = "1.0.0-beta"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsVersionAValid);
            Assert.True(output.IsVersionBValid);
            Assert.Equal(NuGetVersion.Parse(input.VersionA), output.VersionA);
            Assert.Equal(NuGetVersion.Parse(input.VersionB), output.VersionB);
            Assert.Equal(ComparisonResult.GreaterThan, output.Result);
        }

        [Fact]
        public void VersionComparison_Equal()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "1",
                VersionB = "1.0.0.0"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsVersionAValid);
            Assert.True(output.IsVersionBValid);
            Assert.Equal(NuGetVersion.Parse(input.VersionA), output.VersionA);
            Assert.Equal(NuGetVersion.Parse(input.VersionB), output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_InvalidVersionA()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "a",
                VersionB = "1.0.0"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsVersionAValid);
            Assert.True(output.IsVersionBValid);
            Assert.Null(output.VersionA);
            Assert.Equal(NuGetVersion.Parse(input.VersionB), output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_InvalidVersionB()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "1.0.0",
                VersionB = "b"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.True(output.IsVersionAValid);
            Assert.False(output.IsVersionBValid);
            Assert.Equal(NuGetVersion.Parse(input.VersionA), output.VersionA);
            Assert.Null(output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void VersionComparison_InvalidVersionAAndVersionB()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionComparisonInput
            {
                VersionA = "a",
                VersionB = "b"
            };

            // Act
            var output = target.VersionComparison(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsVersionAValid);
            Assert.False(output.IsVersionBValid);
            Assert.Null(output.VersionA);
            Assert.Null(output.VersionB);
            Assert.Equal(ComparisonResult.Equal, output.Result);
        }

        [Fact]
        public void GetNearestFramework_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            GetNearestFrameworkInput input = null;

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Empty(output.Package);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_MissingProject()
        {
            // Arrange
            var target = new ToolsService();
            var inputPackage = new[] { "net45" };
            var input = new GetNearestFrameworkInput
            {
                Project = null,
                Package = string.Join("\n", inputPackage)
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Equal(1, output.Package.Count);
            var outputFramework = output.Package.First();
            Assert.Equal(inputPackage[0], outputFramework.Input);
            Assert.Equal(NuGetFramework.Parse(inputPackage[0]), outputFramework.Framework);
            Assert.False(outputFramework.Compatible);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_MissingPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new GetNearestFrameworkInput
            {
                Project = "net45",
                Package = null
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.True(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Empty(output.Package);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_MissingProjectAndPackage()
        {
            // Arrange
            var target = new ToolsService();
            var input = new GetNearestFrameworkInput
            {
                Project = null,
                Package = null
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Null(output.Project);
            Assert.Empty(output.Package);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_InvalidAndIncompatible()
        {
            // Arrange
            var target = new ToolsService();
            var inputPackage = new[] { "netstandard1.6", "portable-net45+net-cf" };
            var input = new GetNearestFrameworkInput
            {
                Project = "net45",
                Package = string.Join("\n", inputPackage)
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(1, output.Invalid.Count);
            Assert.Equal(inputPackage[1], output.Invalid[0]);
            Assert.True(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Equal(1, output.Package.Count);
            var outputFramework = output.Package.First();
            Assert.Equal(inputPackage[0], outputFramework.Input);
            Assert.Equal(NuGetFramework.Parse(inputPackage[0]), outputFramework.Framework);
            Assert.False(outputFramework.Compatible);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_Mixed()
        {
            // Arrange
            var target = new ToolsService();
            var inputPackage = new[] { "net40", "net45", "netstandard1.6", "portable-net45+net-cf" };
            var input = new GetNearestFrameworkInput
            {
                Project = "net451",
                Package = string.Join("\n", inputPackage)
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(1, output.Invalid.Count);
            Assert.Equal(inputPackage[3], output.Invalid[0]);
            Assert.True(output.IsProjectValid);
            Assert.True(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Equal(3, output.Package.Count);

            var outputFrameworkA = output.Package.ElementAt(0);
            Assert.Equal(inputPackage[0], outputFrameworkA.Input);
            Assert.Equal(NuGetFramework.Parse(inputPackage[0]), outputFrameworkA.Framework);
            Assert.True(outputFrameworkA.Compatible);

            var outputFrameworkB = output.Package.ElementAt(1);
            Assert.Equal(inputPackage[1], outputFrameworkB.Input);
            Assert.Equal(NuGetFramework.Parse(inputPackage[1]), outputFrameworkB.Framework);
            Assert.True(outputFrameworkB.Compatible);

            var outputFrameworkC = output.Package.ElementAt(2);
            Assert.Equal(inputPackage[2], outputFrameworkC.Input);
            Assert.Equal(NuGetFramework.Parse(inputPackage[2]), outputFrameworkC.Framework);
            Assert.False(outputFrameworkC.Compatible);

            Assert.Same(outputFrameworkB, output.Nearest);
        }

        [Fact]
        public void GetNearestFramework_Invalid()
        {
            // Arrange
            var target = new ToolsService();
            var inputPackage = new[] { "portable-net45+net-cf", "portable-win8+net-cf" };
            var input = new GetNearestFrameworkInput
            {
                Project = "net451",
                Package = string.Join("\n", inputPackage)
            };

            // Act
            var output = target.GetNearestFramework(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.Equal(2, output.Invalid.Count);
            Assert.Equal(inputPackage[0], output.Invalid[0]);
            Assert.Equal(inputPackage[1], output.Invalid[1]);
            Assert.True(output.IsProjectValid);
            Assert.False(output.IsPackageValid);
            Assert.Equal(NuGetFramework.Parse(input.Project), output.Project);
            Assert.Equal(0, output.Package.Count);
            Assert.Null(output.Nearest);
        }

        [Fact]
        public void VersionSatisfies_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            VersionSatisfiesInput input = null;

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Null(output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_MissingVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = null,
                Version = "2.0"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Equal(NuGetVersion.Parse(input.Version), output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_MissingVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "(, 1.0.0]",
                Version = null
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.True(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Null(output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_MissingVersionRangeAndVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = null,
                Version = null
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.False(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Null(output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_Satisfies()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "(, 1.0.0]",
                Version = "1.0.0"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Equal(NuGetVersion.Parse(input.Version), output.Version);
            Assert.True(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_DoesNotSatisfy()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "(, 1.0.0)",
                Version = "1.0.0"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.True(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Equal(NuGetVersion.Parse(input.Version), output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_InvalidVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "a",
                Version = "1.0.0"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Equal(NuGetVersion.Parse(input.Version), output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_InvalidVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "1.0.0",
                Version = "b"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.True(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Null(output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void VersionSatisfies_InvalidVersionRangeAndVersion()
        {
            // Arrange
            var target = new ToolsService();
            var input = new VersionSatisfiesInput
            {
                VersionRange = "a",
                Version = "b"
            };

            // Act
            var output = target.VersionSatisfies(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.False(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Null(output.Version);
            Assert.False(output.Satisfies);
        }

        [Fact]
        public void FindBestVersionMatch_NullInput()
        {
            // Arrange
            var target = new ToolsService();
            FindBestVersionMatchInput input = null;

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Empty(output.Versions);
            Assert.Null(output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_MissingVersionRange()
        {
            // Arrange
            var target = new ToolsService();
            var inputVersions = new[] { "1.0.0" };
            var input = new FindBestVersionMatchInput
            {
                VersionRange = null,
                Versions = string.Join("\n", inputVersions)
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Equal(1, output.Versions.Count);
            var outputVersion = output.Versions.First();
            Assert.Equal(inputVersions[0], outputVersion.Input);
            Assert.Equal(NuGetVersion.Parse(inputVersions[0]), outputVersion.Version);
            Assert.False(outputVersion.Satisfies);
            Assert.Null(output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_MissingVersions()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FindBestVersionMatchInput
            {
                VersionRange = "[1.0.0, )",
                Versions = null
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.True(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Empty(output.Versions);
            Assert.Null(output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_MissingVersionRangeAndVersions()
        {
            // Arrange
            var target = new ToolsService();
            var input = new FindBestVersionMatchInput
            {
                VersionRange = null,
                Versions = null
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Missing, output.InputStatus);
            Assert.Empty(output.Invalid);
            Assert.False(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Null(output.VersionRange);
            Assert.Empty(output.Versions);
            Assert.Null(output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_InvalidAndDoesNotSatisfy()
        {
            // Arrange
            var target = new ToolsService();
            var inputVersions = new[] { "0.1.0", "a" };
            var input = new FindBestVersionMatchInput
            {
                VersionRange = "[1.0.0, )",
                Versions = string.Join("\n", inputVersions)
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(1, output.Invalid.Count);
            Assert.Equal(inputVersions[1], output.Invalid[0]);
            Assert.True(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Equal(1, output.Versions.Count);
            var outputVersion = output.Versions.First();
            Assert.Equal(inputVersions[0], outputVersion.Input);
            Assert.Equal(NuGetVersion.Parse(inputVersions[0]), outputVersion.Version);
            Assert.False(outputVersion.Satisfies);
            Assert.Null(output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_Mixed()
        {
            // Arrange
            var target = new ToolsService();
            var inputVersions = new[] { "1.1.0", "1.2.0", "0.9.0", "a" };
            var input = new FindBestVersionMatchInput
            {
                VersionRange = "[1.0.0, 2.0.0]",
                Versions = string.Join("\n", inputVersions)
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Valid, output.InputStatus);
            Assert.Equal(1, output.Invalid.Count);
            Assert.Equal(inputVersions[3], output.Invalid[0]);
            Assert.True(output.IsVersionRangeValid);
            Assert.True(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Equal(3, output.Versions.Count);

            var outputVersionA = output.Versions.ElementAt(0);
            Assert.Equal(inputVersions[0], outputVersionA.Input);
            Assert.Equal(NuGetVersion.Parse(inputVersions[0]), outputVersionA.Version);
            Assert.True(outputVersionA.Satisfies);

            var outputVersionB = output.Versions.ElementAt(1);
            Assert.Equal(inputVersions[1], outputVersionB.Input);
            Assert.Equal(NuGetVersion.Parse(inputVersions[1]), outputVersionB.Version);
            Assert.True(outputVersionB.Satisfies);

            var outputVersionC = output.Versions.ElementAt(2);
            Assert.Equal(inputVersions[2], outputVersionC.Input);
            Assert.Equal(NuGetVersion.Parse(inputVersions[2]), outputVersionC.Version);
            Assert.False(outputVersionC.Satisfies);

            Assert.Same(outputVersionA, output.BestMatch);
        }

        [Fact]
        public void FindBestVersionMatch_Invalid()
        {
            // Arrange
            var target = new ToolsService();
            var inputVersions = new[] { "a", "b" };
            var input = new FindBestVersionMatchInput
            {
                VersionRange = "[1.0.0, )",
                Versions = string.Join("\n", inputVersions)
            };

            // Act
            var output = target.FindBestVersionMatch(input);

            // Assert
            Assert.Same(input, output.Input);
            Assert.Equal(InputStatus.Invalid, output.InputStatus);
            Assert.Equal(2, output.Invalid.Count);
            Assert.Equal(inputVersions[0], output.Invalid[0]);
            Assert.Equal(inputVersions[1], output.Invalid[1]);
            Assert.True(output.IsVersionRangeValid);
            Assert.False(output.IsVersionValid);
            Assert.Equal(VersionRange.Parse(input.VersionRange), output.VersionRange);
            Assert.Equal(0, output.Versions.Count);
            Assert.Null(output.BestMatch);
        }
    }
}
