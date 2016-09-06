using System.Collections.Generic;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public interface IFrameworkEnumerator<TFramework> where TFramework : IFramework
    {
        IEnumerable<FrameworkEnumeratorData<TFramework>> Enumerate(FrameworkEnumerationOptions options);
        IEnumerable<FrameworkEnumeratorData<TFramework>> Expand(IEnumerable<FrameworkEnumeratorData<TFramework>> frameworks, FrameworkExpansionOptions options);
    }
}