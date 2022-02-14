using System;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Website
{
    public static class Configuration
    {
        static Configuration()
        {
            var assembly = typeof(Configuration).GetTypeInfo().Assembly;

            AssemblyVersion = assembly
                .GetCustomAttribute<AssemblyVersionAttribute>()?
                .Version;

            AssemblyFileVersion = assembly
                .GetCustomAttribute<AssemblyFileVersionAttribute>()?
                .Version;

            AssemblyInformationalVersion = assembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            AssemblyCommitHash = assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == "CommitHash")?
                .Value;

            AssemblyBuildTimestamp = DateTimeOffset.Parse(assembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(x => x.Key == "BuildTimestamp")?
                .Value, CultureInfo.InvariantCulture);
        }

        public static string AssemblyVersion { get; private set; }
        public static string AssemblyFileVersion { get; private set; }
        public static string AssemblyInformationalVersion { get; private set; }
        public static string AssemblyCommitHash { get; private set; }
        public static DateTimeOffset AssemblyBuildTimestamp { get; private set; }
    }
}
