using System.IO;
using Microsoft.Extensions.CommandLineUtils;

namespace Knapcode.NuGetTools.Build
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.Command(
                name: "assemblyInfo",
                configuration: assemblyInfoApp =>
                {
                    var baseDirectoryOption = assemblyInfoApp.Option(
                        "--baseDirectory",
                        "The base directory of the NuGetTools repository.",
                        CommandOptionType.SingleValue);

                    assemblyInfoApp.OnExecute(() =>
                        {
                            var baseDirectory = baseDirectoryOption.Value();

                            // Discover the assembly info from Git.
                            var assemblyInfo = AssemblyInfoWriter.DiscoverAssemblyInfo(baseDirectory);

                            // Discover all project.json files.
                            var projects = Directory.EnumerateFiles(
                                baseDirectory,
                                "*project.json",
                                SearchOption.AllDirectories);

                            // Write assembly info for each project.json.
                            foreach (var project in projects)
                            {
                                var projectDirectory = Path.GetDirectoryName(project);
                                var propertiesDirectory = Path.Combine(projectDirectory, "Properties");
                                Directory.CreateDirectory(propertiesDirectory);

                                var assemblyInfoPath = Path.Combine(propertiesDirectory, "AssemblyInfo.cs");
                                AssemblyInfoWriter.WriteAssemblyInfo(assemblyInfoPath, assemblyInfo);
                            }

                            return 0;
                        });
                });

            return app.Execute(args);
        }
    }
}
