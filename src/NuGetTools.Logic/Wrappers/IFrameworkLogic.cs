using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IFrameworkLogic<TFramework>
        where TFramework : IFramework
    {
        TFramework Parse(string input);
        bool IsCompatible(TFramework project, TFramework package);
        TFramework GetNearest(TFramework project, IEnumerable<TFramework> package);
    }
}
