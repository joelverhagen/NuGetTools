namespace Knapcode.NuGetTools.Logic;

public interface IFrameworkEnumerator
{
    IEnumerable<FrameworkEnumeratorData> Enumerate(FrameworkEnumerationOptions options);
    IEnumerable<FrameworkEnumeratorData> Expand(IEnumerable<FrameworkEnumeratorData> frameworks, FrameworkExpansionOptions options);
}
