using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Knapcode.NuGetTools.Logic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;

namespace Knapcode.NuGetTools.Website.Tests;

public class TestServerFixture : IDisposable
{
    public static Uri? BaseAddress { get; set; }

    private readonly HtmlParser _htmlParser;

    public TestServerFixture()
    {
        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder => builder
            .UseWebRoot(GetWebRoot())
            .UseContentRoot(GetContentRoot())
            .UseEnvironment("Automation"));
        Server = Factory.Server;

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

    internal WebApplicationFactory<Program> Factory { get; }
    public TestServer Server { get; }
    public HttpClient Client { get; }

    private static string GetWebRoot()
    {
        return Path.Combine(GetContentRoot(), "wwwroot");
    }

    private static string GetContentRoot()
    {
        return Enumerable
            .Empty<string>()
            .Concat(new[]
            {
                Path.GetDirectoryName(typeof(IntegrationTest).Assembly.Location),
                Directory.GetCurrentDirectory(),
            })
            .Where(x => x is not null)
            .Select(x => GetContentRoot(x!))
            .First(x => x is not null)!;
    }

    private static string? GetContentRoot(string startingDirectory)
    {
        string? currentDirectory = startingDirectory;
        while (currentDirectory != null && !Directory
                .GetFiles(currentDirectory)
                .Select(p => Path.GetFileName(p))
                .Contains("NuGetTools.sln"))
        {
            currentDirectory = Path.GetDirectoryName(currentDirectory);
        }

        if (currentDirectory is null)
        {
            return null;
        }

        return Path.Combine(currentDirectory, "src", "Knapcode.NuGetTools.Website");
    }

    public async Task<List<NuGetVersion>> GetAvailableVersionsAsync()
    {
        var toolsFactory = Factory.Services.GetRequiredService<IToolsFactory>();
        var versionStrings = await toolsFactory.GetAvailableVersionsAsync(CancellationToken.None);
        return versionStrings
            .Select(x => new NuGetVersion(x))
            .ToList();
    }

    public async Task<IHtmlDocument> GetDocumentAsync(HttpResponseMessage response)
    {
        using (var stream = await response.Content.ReadAsStreamAsync())
        {
            return await _htmlParser.ParseDocumentAsync(stream);
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
