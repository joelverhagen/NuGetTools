﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class FrameworkApi3x : IFrameworkApi
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
        private readonly MethodInfo _getIdentifier;
        private readonly MethodInfo _getVersion;
        private readonly MethodInfo _hasProfile;
        private readonly MethodInfo _getProfile;
        private readonly MethodInfo _hasPlatform;
        private readonly MethodInfo _getPlatform;
        private readonly MethodInfo _getPlatformVersion;
        private readonly MethodInfo _getToString;

        public FrameworkApi3x(AssemblyName assemblyName)
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

            _getIdentifier = _nuGetFrameworkType
                .GetProperty("Framework")
                .GetGetMethod();

            _getVersion = _nuGetFrameworkType
                .GetProperty("Version")
                .GetGetMethod();

            _hasProfile = _nuGetFrameworkType
                .GetProperty("HasProfile")
                .GetGetMethod();

            _getProfile = _nuGetFrameworkType
                .GetProperty("Profile")
                .GetGetMethod();

            _hasPlatform = _nuGetFrameworkType
                .GetProperty("HasPlatform")?
                .GetGetMethod();

            _getPlatform = _nuGetFrameworkType
                .GetProperty("Platform")?
                .GetGetMethod();

            _getPlatformVersion = _nuGetFrameworkType
                .GetProperty("PlatformVersion")?
                .GetGetMethod();

            _getToString = _nuGetFrameworkType
                .GetMethod("ToString", new Type[0]);

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

            // CompatibilityProvider
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

        public string GetIdentifer(object nuGetFramework)
        {
            return (string)_getIdentifier.Invoke(nuGetFramework, new object[0]);
        }

        public System.Version GetVersion(object nuGetFramework)
        {
            return (System.Version)_getVersion.Invoke(nuGetFramework, new object[0]);
        }

        public string GetProfile(object nuGetFramework)
        {
            return (string)_getProfile.Invoke(nuGetFramework, new object[0]);
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

        public bool HasProfile(object nuGetFramework)
        {
            return (bool)_hasProfile.Invoke(nuGetFramework, new object[0]);
        }

        public bool HasPlatform(object nuGetFramework)
        {
            if (_hasPlatform == null)
            {
                return false;
            }

            return (bool)_hasPlatform.Invoke(nuGetFramework, new object[0]);
        }

        public string GetPlatform(object nuGetFramework)
        {
            if (_getPlatform == null)
            {
                throw new NotSupportedException();
            }

            return (string)_getPlatform.Invoke(nuGetFramework, new object[0]);
        }

        public System.Version GetPlatformVersion(object nuGetFramework)
        {
            if (_getPlatformVersion == null)
            {
                throw new NotSupportedException();
            }

            return (System.Version)_getPlatformVersion.Invoke(nuGetFramework, new object[0]);
        }

        public string GetToStringResult(object nuGetFramework)
        {
            return (string)_getToString.Invoke(nuGetFramework, new object[0]);
        }
    }
}
