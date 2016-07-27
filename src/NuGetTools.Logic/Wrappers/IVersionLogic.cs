namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IVersionLogic<TVersion>
        where TVersion : IVersion
    {
        TVersion Parse(string input);
        int Compare(TVersion versionA, TVersion versionB);
    }
}
