namespace Knapcode.NuGetTools.Logic.Wrappers;

public interface IFrameworkLogic
{
    IFramework Parse(string input);
    bool IsCompatible(IFramework project, IFramework package);
    IFramework? GetNearest(IFramework project, IEnumerable<IFramework> package);
}
