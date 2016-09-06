using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic
{
    public interface IFrameworkList
    {
        IReadOnlyList<string> DotNetFrameworkNames { get; }
    }
}
