using NuGet.Frameworks;
using NuGet.Versioning;
using Xunit;

namespace Knapcode.NuGetTools.Website.Tests
{
    public class ToolsServiceTests
    {
        [Fact]
        public void ParseVersionRange_Valid()
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
        public void ParseVersionRange_Invalid()
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
        public void ParseVersionRange_Missing()
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
        public void ParseVersion_Valid()
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
        public void ParseVersion_Invalid()
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
        public void ParseVersion_Missing()
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
        public void ParseFramework_ValidFrameworkName()
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
        public void ParseFramework_InvalidFrameworkName()
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
        public void ParseFramework_MissingInput()
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
    }
}
