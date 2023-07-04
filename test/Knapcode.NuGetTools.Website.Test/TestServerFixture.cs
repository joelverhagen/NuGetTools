using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

namespace Knapcode.NuGetTools.Website.Tests
{
    public class TestServerFixture : IDisposable
    {
        public static Uri BaseAddress { get; set; }

        private readonly HtmlParser _htmlParser;

        public TestServerFixture()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseWebRoot(GetWebRoot())
                .UseContentRoot(GetContentRoot())
                .UseEnvironment("Automation");
            Server = new TestServer(webHostBuilder);

            if (BaseAddress == null)
            {
                Client = Server.CreateClient();
            }
            else
            {
                var handler = new HttpClientHandler
                {
                    AllowAutoRedirect = false,
                };
                Client = new HttpClient(handler)
                {
                    BaseAddress = BaseAddress,
                };
            }

            _htmlParser = new HtmlParser();
        }

        public TestServer Server { get; }
        public HttpClient Client { get; }

        private static string GetWebRoot()
        {
            return Path.Combine(GetContentRoot(), "wwwroot");
        }

        private static string GetContentRoot()
        {
            return Enumerable.Empty<string>()
                .Concat(new[]
                {
                        Path.GetDirectoryName(typeof(IntegrationTest).Assembly.Location),
                        Directory.GetCurrentDirectory(),
                })
                .Select(x => GetContentRoot(x))
                .FirstOrDefault(x => x != null);
        }

        private static string GetContentRoot(string currentDirectory)
        {
            while (currentDirectory != null && !Directory
                    .GetFiles(currentDirectory)
                    .Select(p => Path.GetFileName(p))
                    .Contains("NuGetTools.sln"))
            {
                currentDirectory = Path.GetDirectoryName(currentDirectory);
            }

            if (currentDirectory == null)
            {
                return null;
            }

            return Path.Combine(currentDirectory, "src", "Knapcode.NuGetTools.Website");
        }

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

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}
