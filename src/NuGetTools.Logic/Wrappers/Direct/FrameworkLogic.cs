using System.Collections.Generic;
using System.Linq;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Logic.Wrappers.Direct
{
    public class FrameworkLogic : IFrameworkLogic<Framework>
    {
        private readonly FrameworkReducer _reducer;
        private readonly IFrameworkCompatibilityProvider _compatibilityProvider;

        public FrameworkLogic()
        {
            _reducer = new FrameworkReducer();
            _compatibilityProvider = DefaultCompatibilityProvider.Instance;
        }

        public Framework GetNearest(Framework project, IEnumerable<Framework> package)
        {
            var nearest = _reducer.GetNearest(
                project.NuGetFramework,
                package.Select(x => x.NuGetFramework));

            if (nearest == null)
            {
                return null;
            }

            return package.First(x => ReferenceEquals(x.NuGetFramework, nearest));
        }

        public bool IsCompatible(Framework project, Framework package)
        {
            return _compatibilityProvider.IsCompatible(
                project.NuGetFramework,
                package.NuGetFramework);
        }

        public Framework Parse(string input)
        {
            var nuGetFramework = NuGetFramework.Parse(input);
            return new Framework(nuGetFramework);
        }
    }
}
