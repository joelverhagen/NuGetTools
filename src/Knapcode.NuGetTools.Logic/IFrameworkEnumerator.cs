using System.Collections.Generic;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public interface IFrameworkEnumerator<TFramework> where TFramework : IFramework
    {
        IEnumerable<FrameworkData<TFramework>> Enumerate(FrameworkEnumerationOptions options);
        IEnumerable<FrameworkData<TFramework>> Expand(IEnumerable<FrameworkData<TFramework>> frameworks, FrameworkExpansionOptions options);
    }
}