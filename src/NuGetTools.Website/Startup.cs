using System.IO;
using System.Linq;
using System.Reflection;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Knapcode.NuGetTools.Website
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IToolsFactory>(serviceProvider =>
            {
                var assemblyLoader = new AssemblyLoader();
                var context = assemblyLoader.GetAppDomainContext("NuGet");

                var assemblyLocation = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                assemblyLoader.LoadAssembly(context, Path.Combine(assemblyLocation, "NuGet.Versioning.dll"));
                assemblyLoader.LoadAssembly(context, Path.Combine(assemblyLocation, "NuGet.Frameworks.dll"));

                var frameworksAssemblyName = context.LoadedAssemblies.First(x => x.Name == "NuGet.Frameworks");
                var versioningAssemblyName = context.LoadedAssemblies.First(x => x.Name == "NuGet.Versioning");

                var frameworkLogic = context.Proxy.GetFrameworkLogic(frameworksAssemblyName);
                var versionLogic = context.Proxy.GetVersionLogic(versioningAssemblyName);
                var versionRangeLogic = context.Proxy.GetVersionRangeLogic(versioningAssemblyName);

                var toolsService = new ToolsService<Framework, Version, VersionRange>(
                    "3.5.0-beta2",
                    frameworkLogic,
                    versionLogic,
                    versionRangeLogic);

                var toolsFactory = new SingletonToolsFactory(toolsService);

                return toolsFactory;
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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
