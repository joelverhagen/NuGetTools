using System.Runtime.Versioning;
using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x;

public class FrameworkLogic2x : IFrameworkLogic
{
    public IFramework? GetNearest(IFramework project, IEnumerable<IFramework> package)
    {
        var prsList = new List<PackageReferenceSet>();
        var prsToFramework = new Dictionary<PackageReferenceSet, Framework2x>();
        foreach (var o in package)
        {
            var framework = (Framework2x)o;
            var prs = new PackageReferenceSet(framework.FrameworkName, Enumerable.Empty<string>());
            prsList.Add(prs);
            prsToFramework[prs] = framework;
        }

        var matched = VersionUtility.TryGetCompatibleItems(((Framework2x)project).FrameworkName, prsList, out var compatible);
        if (!matched)
        {
            return null;
        }

        var first = compatible.First();
        return prsToFramework[first];
    }

    public bool IsCompatible(IFramework project, IFramework package)
    {
        return VersionUtility.IsCompatible(
            ((Framework2x)project).FrameworkName,
            new FrameworkName[] { ((Framework2x)project).FrameworkName });
    }

    public IFramework Parse(string input)
    {
        if (input.Contains(','))
        {
            return new Framework2x(new FrameworkName(input));
        }

        return new Framework2x(VersionUtility.ParseFrameworkName(input));
    }
}
