using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class FrameworkApi
    {
        private readonly MethodInfo _getDotNetFrameworkName;
        private readonly MethodInfo _getShortFolderName;
        private readonly MethodInfo _parse;
        private readonly MethodInfo _getNearest;
        private readonly MethodInfo _isCompatible;
        private readonly MethodInfo _getCompatibilityProviderInstance;
        private readonly Type _frameworkReducerType;
        private readonly Type _nuGetFrameworkType;
        private readonly Type _nuGetFrameworkListType;
        private readonly MethodInfo _listAdd;

        public FrameworkApi(AssemblyName assemblyName)
        {
            var assembly = assemblyName.GetAssembly();

            // NuGetFramework
            _nuGetFrameworkType = assembly.GetType("NuGet.Frameworks.NuGetFramework");

            _getDotNetFrameworkName = _nuGetFrameworkType
                .GetProperty("DotNetFrameworkName")
                .GetGetMethod();

            _getShortFolderName = _nuGetFrameworkType
                .GetMethod("GetShortFolderName", new Type[0]);

            _parse = _nuGetFrameworkType
                .GetMethod("Parse", new[] { typeof(string) });

            // List<NuGetFramework>
            var listType = typeof(List<>);
            _nuGetFrameworkListType = listType.MakeGenericType(_nuGetFrameworkType);

            _listAdd = _nuGetFrameworkListType
                .GetMethod("Add");

            // FrameworkReducer
            _frameworkReducerType = assembly.GetType("NuGet.Frameworks.FrameworkReducer");
            
            _getNearest = _frameworkReducerType
                .GetMethod("GetNearest");

            // DefaultCompatibilityProvider
            var defaultCompatibilityProviderType = assembly.GetType("NuGet.Frameworks.DefaultCompatibilityProvider");

            _getCompatibilityProviderInstance = defaultCompatibilityProviderType
                .GetProperty("Instance")
                .GetGetMethod();

            // CompatiblityProvider
            var compatibilityProviderType = assembly.GetType("NuGet.Frameworks.CompatibilityProvider");

            _isCompatible = compatibilityProviderType
                .GetMethod("IsCompatible");
        }

        public string GetDotNetFrameworkName(object nuGetFramework)
        {
            return (string)_getDotNetFrameworkName.Invoke(nuGetFramework, new object[0]);
        }

        public string GetShortFolderName(object nuGetFramework)
        {
            return (string)_getShortFolderName.Invoke(nuGetFramework, new object[0]);
        }

        public object Parse(string input)
        {
            return _parse.Invoke(null, new[] { input });
        }

        public object GetNearest(object project, IEnumerable<object> package)
        {
            var reducer = Activator.CreateInstance(_frameworkReducerType);

            var packageList = Activator.CreateInstance(_nuGetFrameworkListType);
            foreach (var o in package)
            {
                _listAdd.Invoke(packageList, new[] { o });
            }

            return _getNearest.Invoke(reducer, new[] { project, packageList });
        }

        public bool IsCompatible(object project, object package)
        {
            var compatibilityProvider = _getCompatibilityProviderInstance.Invoke(null, new object[0]);

            return (bool)_isCompatible.Invoke(compatibilityProvider, new[] { project, package });
        }
    }
}
