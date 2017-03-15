using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using Knapcode.NuGetTools.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;
using Xunit;

namespace Knapcode.NuGetTools.Website.Tests
{
    public class IntegrationTest
    {
        public static readonly List<NuGetVersion> AvailableVersions;
        public static readonly TheoryData<NuGetVersion> AvailableVersionData;

        static IntegrationTest()
        {
            using (var tc = new TestContext())
            {
                AvailableVersions = tc
                    .GetAvailableVersionsAsync()
                    .Result
                    .OrderBy(x => x)
                    .ToList();
            }

            AvailableVersionData = new TheoryData<NuGetVersion>();
            foreach (var version in AvailableVersions)
            {
                AvailableVersionData.Add(version);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task ParseFramework(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/parse-framework?framework=net45";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("The input value is net45.", text);
                Assert.Contains("The short folder name is net45.", text);
                Assert.Contains("The .NET framework name is .NETFramework,Version=v4.5.", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task FrameworkCompatibility(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/framework-compatibility?project=net46&package=net45";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("net46 (net46) projects support net45 (net45) packages.", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task GetNearestFramework(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/get-nearest-framework?project=net46&package=net40%0D%0Anet45%0D%0Anet461";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("The net45 (net45) package framework is the nearest to the net46 (net46) project framework.", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task FrameworkPrecedence(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/framework-precedence?framework=net45";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("The .NETFramework,Version=v4.5 (net45) project framework has the following package framework precedence list.", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task ParseVersion(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/parse-version?version=1.0.0-beta01";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("The result of ToString() is 1.0.0-beta01.", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task VersionComparison(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/version-comparison?versionA=1.0.0-beta&versionB=2.0.0";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("1.0.0-beta (1.0.0-beta) is less than 2.0.0 (2.0.0).", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task ParseVersionRange(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/parse-version-range?versionRange=%5B1.0.0%2C+2.0.0%5D";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("The normalized version range is [1.0.0, 2.0.0].", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task VersionSatisfies(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/version-satisfies?versionRange=%5B1.0.0%2C+2.0.0%5D&version=1.5.0";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                Assert.Contains("1.5.0 (1.5.0) satisfies [1.0.0, 2.0.0] ([1.0.0, 2.0.0]).", text);
            }
        }

        [Theory]
        [MemberData(nameof(AvailableVersionData))]
        public async Task FindBestVersionMatch(NuGetVersion version)
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var requestUri = $"/{version}/find-best-version-match?versionRange=%5B1.0.0%2C+2.0.0%5D&versions=0.9.0%0D%0A1.5.0%0D%0A2.1.0";

                // Act
                var response = await tc.Client.GetAsync(requestUri);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var text = await tc.GetFlattenedTextAsync(response);
                if (version.Major >= 3)
                {
                    Assert.Contains("The 1.5.0 (1.5.0) version is the best match to the [1.0.0, 2.0.0] ([1.0.0, 2.0.0]) version range.", text);
                }
                else
                {
                    Assert.Contains("Finding the best version match is only supported in NuGet 3.x and greater.", text);
                }
            }
        }

        [Fact]
        public async Task RootRedirectsToLatestVersion()
        {
            // Arrange
            using (var tc = new TestContext())
            {
                var maxVersion = AvailableVersions.Max();

                // Act
                var response = await tc.Client.GetAsync("/");

                // Assert
                Assert.Equal(HttpStatusCode.Found, response.StatusCode);
                Assert.EndsWith($"/{maxVersion}", response.Headers.Location.ToString());
            }
        }

        private class TestContext : IDisposable
        {
            private readonly HtmlParser _htmlParser;

            public TestContext()
            {
                var webHostBuilder = new WebHostBuilder()
                    .UseStartup<Startup>()
                    .UseWebRoot(GetWebRoot())
                    .UseContentRoot(GetContentRoot())
                    .UseEnvironment("Automation");
                Server = new TestServer(webHostBuilder);
                Client = Server.CreateClient();

                _htmlParser = new HtmlParser();
            }

            public TestServer Server { get; }
            public HttpClient Client { get; }

            public async Task<List<NuGetVersion>> GetAvailableVersionsAsync()
            {
                var toolsFactory = Server.Host.Services.GetRequiredService<IToolsFactory>();
                var versionStrings = await toolsFactory.GetAvailableVersionsAsync(CancellationToken.None);
                return versionStrings
                    .Select(x => new NuGetVersion(x))
                    .ToList();
            }

            public async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return await _htmlParser.ParseAsync(stream);
                }
            }

            public async Task<string> GetFlattenedTextAsync(HttpResponseMessage response)
            {
                var document = await GetDocumentAsync(response);
                var textContent = document.DocumentElement.Text();
                return Regex.Replace(textContent, @"\s+", " ", RegexOptions.Multiline);
            }

            private static string GetWebRoot()
            {
                return Path.Combine(GetContentRoot(), "wwwroot");
            }

            private static string GetContentRoot()
            {
                var currentDirectory = Path.GetDirectoryName(typeof(IntegrationTest).Assembly.Location);
                while (currentDirectory != null && !Directory
                        .GetFiles(currentDirectory)
                        .Select(p => Path.GetFileName(p))
                        .Contains("NuGetTools.sln"))
                {
                    currentDirectory = Path.GetDirectoryName(currentDirectory);
                }

                return Path.Combine(currentDirectory, "src", "Knapcode.NuGetTools.Website");
            }

            public void Dispose()
            {
                Client.Dispose();
                Server.Dispose();
            }
        }
    }
}
