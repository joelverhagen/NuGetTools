namespace Knapcode.NuGetTools.Logic
{
    public interface IFrameworkList
    {
        IReadOnlyList<string> DotNetFrameworkNames { get; }
        IReadOnlyList<string> ShortFolderNames { get; }
        IReadOnlyList<string> Identifiers { get; }
    }
}
