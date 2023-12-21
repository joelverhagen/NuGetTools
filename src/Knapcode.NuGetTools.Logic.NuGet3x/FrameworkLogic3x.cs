using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Logic.NuGet3x;

public class FrameworkLogic3x : IFrameworkLogic
{
    private readonly FrameworkReducer _reducer;
    private readonly IFrameworkCompatibilityProvider _compatibilityProvider;

    public FrameworkLogic3x()
    {
        _reducer = new FrameworkReducer();
        _compatibilityProvider = DefaultCompatibilityProvider.Instance;
    }

    public IFramework? GetNearest(IFramework project, IEnumerable<IFramework> package)
    {
        var nearest = _reducer.GetNearest(
            ((Framework3x)project).NuGetFramework,
            package.Cast<Framework3x>().Select(x => x.NuGetFramework));

        if (nearest is null)
        {
            return null;
        }

        return package.First(x => ReferenceEquals(((Framework3x)x).NuGetFramework, nearest));
    }

    public bool IsCompatible(IFramework project, IFramework package)
    {
        return _compatibilityProvider.IsCompatible(
            ((Framework3x)project).NuGetFramework,
            ((Framework3x)package).NuGetFramework);
    }

    public IFramework Parse(string input)
    {
        var nuGetFramework = NuGetFramework.Parse(input);
        return new Framework3x(nuGetFramework);
    }
}
