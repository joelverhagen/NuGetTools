using System;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class ReflectionFramework : MarshalByRefObject, IFramework
    {
        private readonly FrameworkApi _api;

        public ReflectionFramework(object nuGetFramework, FrameworkApi api)
        {
            NuGetFramework = nuGetFramework;
            _api = api;
        }

        public object NuGetFramework { get; }
        public string DotNetFrameworkName => _api.GetDotNetFrameworkName(NuGetFramework);
        public string ShortFolderName => _api.GetShortFolderName(NuGetFramework);
    }
}
