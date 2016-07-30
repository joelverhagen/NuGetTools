using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class ReflectionFrameworkLogic : MarshalByRefObject, IFrameworkLogic<ReflectionFramework>
    {
        private readonly FrameworkApi _api;

        public ReflectionFrameworkLogic(FrameworkApi api)
        {
            _api = api;
        }

        public ReflectionFramework GetNearest(ReflectionFramework project, IEnumerable<ReflectionFramework> package)
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

        public bool IsCompatible(ReflectionFramework project, ReflectionFramework package)
        {
            return _api.IsCompatible(
                project.NuGetFramework,
                package.NuGetFramework);
        }

        public ReflectionFramework Parse(string input)
        {
            var nuGetFramework = _api.Parse(input);

            return new ReflectionFramework(nuGetFramework, _api);
        }
    }
}
