using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Knapcode.NuGetTools.Build
{
    public static class AssemblyInfoWriter
    {
        private const string AssemblyInfoFormat = @"using System.Reflection;

[assembly: AssemblyFileVersion(""{0}"")]
[assembly: AssemblyVersion(""{1}"")]
[assembly: AssemblyInformationalVersion(""{2}"")]
[assembly: AssemblyMetadata(""CommitHash"", ""{3}"")]
[assembly: AssemblyMetadata(""BuildTimestamp"", ""{4:O}"")]
";

        public static AssemblyInfo DiscoverAssemblyInfo(string repositoryDirectory, string versionSuffix)
        {
            var rawAssemblyVersion = GetGitOutput("describe --abbrev=0");

            // Remove any prerelease label.
            rawAssemblyVersion = rawAssemblyVersion.Split('-')[0];

            // Make sure there are four digits in the version number.
            while (rawAssemblyVersion.Count(x => x == '.') < 3)
            {
                rawAssemblyVersion += ".0";
            }

            var version = new Version(rawAssemblyVersion);

            var informationalVersion = GetGitOutput("describe --long --dirty");
            informationalVersion += versionSuffix;

            var commitHash = GetGitOutput("rev-parse HEAD");

            var assemblyInfo = new AssemblyInfo(
                version,
                version,
                informationalVersion,
                commitHash,
                DateTimeOffset.UtcNow);

            return assemblyInfo;
        }

        public static void WriteAssemblyInfo(string path, AssemblyInfo assemblyInfo)
        {
            var assemblyInfoBytes = GetFileContent(assemblyInfo);

            // Only write the file if it has changed.
            if (File.Exists(path))
            {
                var existingBytes = File.ReadAllBytes(path);
                if (existingBytes.SequenceEqual(assemblyInfoBytes))
                {
                    return;
                }
            }

            File.WriteAllBytes(path, assemblyInfoBytes);
        }

        private static string GetGitOutput(string arguments)
        {
            var commandRunner = new CommandRunner();
            var command = new Command("git", arguments);
            var result = commandRunner.Run(command);

            result.EnsureSuccess();

            return result.Output.Trim();
        }

        private static byte[] GetFileContent(AssemblyInfo assemblyInfo)
        {
            var content = string.Format(
                AssemblyInfoFormat,
                assemblyInfo.FileVersion,
                assemblyInfo.Version,
                assemblyInfo.InformationalVersion,
                assemblyInfo.CommitHash,
                assemblyInfo.BuildTimestamp);

            return Encoding.UTF8.GetBytes(content);
        }
    }
}
