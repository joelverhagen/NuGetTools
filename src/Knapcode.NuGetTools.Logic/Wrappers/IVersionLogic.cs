namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IVersionLogic
    {
        IVersion Parse(string input);
        int Compare(IVersion versionA, IVersion versionB);
    }
}
