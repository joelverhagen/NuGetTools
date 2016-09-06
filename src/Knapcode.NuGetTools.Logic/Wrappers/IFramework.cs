namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IFramework
    {
        string ShortFolderName { get; }
        string DotNetFrameworkName { get; }
        string Identifier { get; }
        System.Version Version { get; }
        string Profile { get; }
    }
}
