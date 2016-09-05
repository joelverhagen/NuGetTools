namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IFramework
    {
        string ShortFolderName { get; }
        string DotNetFrameworkName { get; }
    }
}
