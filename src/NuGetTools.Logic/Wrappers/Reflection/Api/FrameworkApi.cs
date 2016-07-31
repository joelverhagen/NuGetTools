using System;
using System.Collections.Generic;
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
            // NuGetFramework
            var nuGetFrameworkTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Frameworks.NuGetFramework");

            _nuGetFrameworkType = Type.GetType(nuGetFrameworkTypeName);

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
            var frameworkReducerTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Frameworks.FrameworkReducer");

            _frameworkReducerType = Type.GetType(frameworkReducerTypeName);
            
            _getNearest = _frameworkReducerType
                .GetMethod("GetNearest");

            // DefaultCompatibilityProvider
            var defaultCompatibilityProviderTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Frameworks.DefaultCompatibilityProvider");

            var defaultCompatibilityProviderType = Type.GetType(defaultCompatibilityProviderTypeName);

            _getCompatibilityProviderInstance = defaultCompatibilityProviderType
                .GetProperty("Instance")
                .GetGetMethod();

            // CompatiblityProvider
            var compatiblityProviderTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Frameworks.CompatibilityProvider");

            var compatibilityProviderType = Type.GetType(compatiblityProviderTypeName);

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
