using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic;
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

        services.AddSingleton<IToolsFactory, ToolsFactory>();

        try
        {
            // Try to construct the reflection-based tools factory.
            var serviceProvider = services.BuildServiceProvider();
            var toolsFactory = serviceProvider.GetRequiredService<IToolsFactory>();

            var versions = toolsFactory.GetAvailableVersionsAsync(CancellationToken.None).Result;

            if (!versions.Any())
            {
                throw new InvalidOperationException("At least one version is required.");
            }
        }
        catch
        {
            // Fallback to using the NuGet version directly referenced by this project.
            var serviceDescriptor = services.First(x => x.ImplementationType == typeof(ToolsFactory));
            services.Remove(serviceDescriptor);

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

            services.AddSingleton<IToolsFactory, SingletonToolsFactory>();
        }

        return services;
    }
}
