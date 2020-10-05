namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IFramework
    {
        string ShortFolderName { get; }
        string DotNetFrameworkName { get; }
        string Identifier { get; }
        System.Version Version { get; }
        bool HasProfile { get; }
        string Profile { get; }
        bool HasPlatform { get; }
        string Platform { get; }
        System.Version PlatformVersion { get; }
        string ToStringResult { get; }
    }
}
