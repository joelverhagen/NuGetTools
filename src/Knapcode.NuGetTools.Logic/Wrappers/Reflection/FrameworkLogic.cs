using System;
using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class FrameworkLogic : MarshalByRefObject, IFrameworkLogic<Framework>
    {
        private readonly IFrameworkApi _api;

        public FrameworkLogic(IFrameworkApi api)
        {
            _api = api;
        }

        public Framework GetNearest(Framework project, IEnumerable<Framework> package)
        {
            var nearest = _api.GetNearest(
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
            return _api.IsCompatible(
                project.NuGetFramework,
                package.NuGetFramework);
        }

        public Framework Parse(string input)
        {
            var nuGetFramework = _api.Parse(input);

            return new Framework(nuGetFramework, _api);
        }
        
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
