using System;
using System.IO;
using System.Linq;
using System.Threading;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic.Direct.Wrappers;
using Knapcode.NuGetTools.Logic.Wrappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using Version = Knapcode.NuGetTools.Logic.Direct.Wrappers.Version;

namespace Knapcode.NuGetTools.Website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            HostingEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IHostingEnvironment HostingEnvironment { get; }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var hostingEnvironment = serviceProvider.GetService<IHostingEnvironment>();

                var settings = new InMemorySettings();
                var nuGetSettings = new NuGetSettings(settings);
                nuGetSettings.GlobalPackagesFolder = Path.Combine(hostingEnvironment.ContentRootPath, "packages");

                return nuGetSettings;
            });

            services.AddTransient<IAssemblyLoader, AssemblyLoader>();
            services.AddTransient<IPackageLoader, PackageLoader>();
            services.AddTransient<IPackageRangeDownloader, PackageRangeDownloader>();
            services.AddTransient<IAlignedVersionsDownloader, AlignedVersionsDownloader>();

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

                services.AddTransient<IFrameworkLogic<Framework>, FrameworkLogic>();
                services.AddTransient<IVersionLogic<Version>, VersionLogic>();
                services.AddTransient<IVersionRangeLogic<Version, VersionRange>, VersionRangeLogic>();

                services.AddTransient<IToolsService>(serviceProvider =>
                {
                    return new ToolsService<Framework, Version, VersionRange>(
                        ClientVersionUtility.GetNuGetAssemblyVersion(),
                        serviceProvider.GetRequiredService<IFrameworkLogic<Framework>>(),
                        serviceProvider.GetRequiredService<IVersionLogic<Version>>(),
                        serviceProvider.GetRequiredService<IVersionRangeLogic<Version, VersionRange>>());
                });

                services.AddSingleton<IToolsFactory, SingletonToolsFactory>();
            }

            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            // Add Application Insights monitoring to the request pipeline as a very first middleware.
            app.UseApplicationInsightsRequestTelemetry();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Add Application Insights exceptions handling to the request pipeline.
            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
