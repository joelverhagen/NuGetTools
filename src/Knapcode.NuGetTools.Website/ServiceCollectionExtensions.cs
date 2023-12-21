using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic.NuGet3x;
using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Common;
using NuGet.Versioning;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddNuGetTools(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            var hostingEnvironment = serviceProvider.GetRequiredService<IHostEnvironment>();

            var settings = new InMemorySettings();
            var nuGetSettings = new NuGetSettings(settings);
            nuGetSettings.GlobalPackagesFolder = Path.Combine(hostingEnvironment.ContentRootPath, "packages");

            return nuGetSettings;
        });

        services.AddTransient<IAlignedVersionsDownloader, AlignedVersionsDownloader>();
        services.AddTransient<IFrameworkEnumerator, FrameworkEnumerator>();
        services.AddTransient<IFrameworkList, FrameworkList>();
        services.AddTransient<IPackageRangeDownloader, PackageRangeDownloader>();

        services.AddSingleton<VersionedToolsFactory>();

        AddDirectToolsServices(services);

        services.AddSingleton<IToolsFactory>(serviceProvider =>
        {
            var versioned = GetVersionedToolsFactory(serviceProvider);
            if (versioned is not null)
            {
                return versioned;
            }

            return serviceProvider.GetRequiredService<DirectToolsFactory>();
        });

        return services;
    }

    private static void AddDirectToolsServices(IServiceCollection services)
    {
        services.AddTransient<IFrameworkLogic, FrameworkLogic3x>();
        services.AddTransient<IVersionLogic, VersionLogic3x>();
        services.AddTransient<IVersionRangeLogic, VersionRangeLogic3x>();
        services.AddTransient<INuGetLogic, NuGetLogic3x>();

        var clientVersion = NuGetVersion.Parse(ClientVersionUtility.GetNuGetAssemblyVersion()).ToNormalizedString();

        services.AddTransient<IToolsService, ToolsService>(serviceProvider =>
        {
            return new ToolsService(
                clientVersion,
                serviceProvider.GetRequiredService<INuGetLogic>());
        });

        services.AddTransient<IFrameworkPrecedenceService>(serviceProvider =>
        {
            return new FrameworkPrecedenceService(
                clientVersion,
                serviceProvider.GetRequiredService<IFrameworkList>(),
                serviceProvider.GetRequiredService<IFrameworkLogic>());
        });

        services.AddSingleton<DirectToolsFactory>();
    }

    private static VersionedToolsFactory? GetVersionedToolsFactory(IServiceProvider serviceProvider)
    {
        var settings = serviceProvider.GetRequiredService<NuGetSettings>();
        if (!Directory.Exists(settings.GlobalPackagesFolder)
            || !Directory.EnumerateDirectories(settings.GlobalPackagesFolder).Any())
        {
            return null;
        }

        var factory = serviceProvider.GetRequiredService<VersionedToolsFactory>();
        var versions = factory.GetAvailableVersionsAsync(CancellationToken.None).Result;
        if (!versions.Any())
        {
            return null;
        }

        return factory;
    }
}
