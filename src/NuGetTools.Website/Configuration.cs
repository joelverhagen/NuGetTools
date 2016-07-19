using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Website
{
    public static class Configuration
    {
        private const string Prefix = "NuGetTools.Website.";

        static Configuration()
        {
            try
            {
                var versionLines = GetResourceLines("version.txt");

                if (versionLines.Count < 3)
                {
                    return;
                }

                var rawVersion = versionLines[0].Trim();
                while (rawVersion.Count(c => c == '.') < 3)
                {
                    rawVersion += ".0";
                }

                AssemblyVersion = new Version(rawVersion);
                AssemblyFileVersion = AssemblyFileVersion;
                AssemblyInformationalVersion = versionLines[1].Trim();
                AssemblyCommitHash = versionLines[2].Trim();
            }
            catch
            {
                AssemblyVersion = new Version("0.0.0.0");
                AssemblyFileVersion = AssemblyVersion;
                AssemblyInformationalVersion = "0.0.0";
                AssemblyCommitHash = "0000000000000000000000000000000000000000";
            }   
        }

        public static Version AssemblyVersion { get; private set; }
        public static Version AssemblyFileVersion { get; private set; }
        public static string AssemblyInformationalVersion { get; private set; }
        public static string AssemblyCommitHash { get; private set; }

        private static IReadOnlyList<string> GetResourceLines(string name)
        {
            var lines = new List<string>();
            using (var stream = GetResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            return lines;
        }

        private static Stream GetResourceStream(string name)
        {
            var fullName = Prefix + name.Replace('/', '.');
            var assembly = typeof(Configuration).GetTypeInfo().Assembly;

            return assembly.GetManifestResourceStream(fullName);
        }
    }
}
