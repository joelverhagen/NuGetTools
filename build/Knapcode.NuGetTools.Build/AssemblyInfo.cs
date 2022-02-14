using System;

namespace Knapcode.NuGetTools.Build
{
    public class AssemblyInfo
    {
        public AssemblyInfo(
            Version fileVersion,
            Version version,
            string informationalVersion,
            string commitHash,
            DateTimeOffset buildTimestamp)
        {
            FileVersion = fileVersion;
            Version = version;
            InformationalVersion = informationalVersion;
            CommitHash = commitHash;
            BuildTimestamp = buildTimestamp;
        }

        public Version FileVersion { get; }
        public Version Version { get; }
        public string InformationalVersion { get; }
        public string CommitHash { get; }
        public DateTimeOffset BuildTimestamp { get; }
    }
}
