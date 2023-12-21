using System.Runtime.Versioning;
using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x;

public class Framework2x : IFramework
{
    public Framework2x(FrameworkName frameworkName)
    {
        FrameworkName = frameworkName;
    }

    public FrameworkName FrameworkName { get; }

    public string ShortFolderName => VersionUtility.GetShortFrameworkName(FrameworkName);
    public string DotNetFrameworkName => FrameworkName.ToString();
    public string Identifier => FrameworkName.Identifier;
    public Version Version => FrameworkName.Version;
    public bool HasProfile => !string.IsNullOrEmpty(FrameworkName.Profile);
    public string Profile => FrameworkName.Profile;
    public bool IsPlatformAvailable => false;
    public bool HasPlatform => throw new NotSupportedException();
    public string Platform => throw new NotSupportedException();
    public Version PlatformVersion => throw new NotSupportedException();
    public string ToStringResult => FrameworkName.ToString();
}
