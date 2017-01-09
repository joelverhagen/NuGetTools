using System;
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
                name: "assembly-info",
                configuration: commandApp =>
                {
                    var baseDirectoryOption = commandApp.Option(
                        "--base-directory",
                        "The base directory of the NuGetTools repository.",
                        CommandOptionType.SingleValue);

                    var versionSuffixOption = commandApp.Option(
                        "--version-suffix",
                        "The suffix to add to the information version.",
                        CommandOptionType.SingleValue);

                    commandApp.OnExecute(() =>
                    {
                        var baseDirectory = baseDirectoryOption.Value();
                        var versionSuffix = versionSuffixOption.Value();

                        ExecuteAssemblyInfo(baseDirectory, versionSuffix);

                        return 0;
                    });
                });

            return app.Execute(args);
        }

        private static void ExecuteAssemblyInfo(string baseDirectory, string versionSuffix)
        {
            // Discover the assembly info from Git.
            var assemblyInfo = AssemblyInfoWriter.DiscoverAssemblyInfo(baseDirectory, versionSuffix);

            Console.WriteLine("AssemblyInfo:");
            Console.WriteLine($"  FileVersion:          {assemblyInfo.FileVersion}");
            Console.WriteLine($"  Version:              {assemblyInfo.Version}");
            Console.WriteLine($"  InformationalVersion: {assemblyInfo.InformationalVersion}");
            Console.WriteLine($"  CommitHash:           {assemblyInfo.CommitHash}");

            // Discover all project.json files.
            var projects = Directory.EnumerateFiles(
                baseDirectory,
                "*project.json",
                SearchOption.AllDirectories);

            // Write assembly info for each project.json.
            Console.WriteLine("Paths:");
            foreach (var project in projects)
            {
                var projectDirectory = Path.GetDirectoryName(project);
                var propertiesDirectory = Path.Combine(projectDirectory, "Properties");
                var assemblyInfoPath = Path.Combine(propertiesDirectory, "AssemblyInfo.cs");
                Console.WriteLine($"- {MakeRelativePath(baseDirectory, assemblyInfoPath)}");

                Directory.CreateDirectory(propertiesDirectory);
                AssemblyInfoWriter.WriteAssemblyInfo(assemblyInfoPath, assemblyInfo);
            }
        }

        private static string MakeRelativePath(string baseDirectory, string path)
        {
            var baseDirectoryUri = new Uri(baseDirectory + Path.DirectorySeparatorChar);
            var pathUri = new Uri(path);
            var relativeUri = baseDirectoryUri.MakeRelativeUri(pathUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
