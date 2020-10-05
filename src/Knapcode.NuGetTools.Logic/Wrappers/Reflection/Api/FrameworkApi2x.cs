using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class FrameworkApi2x : IFrameworkApi
    {
        private readonly MethodInfo _getShortFolderName;
        private readonly MethodInfo _parse;
        private readonly MethodInfo _isCompatible;
        private readonly MethodInfo _tryGetCompatibleItems;
        private readonly Type _packageReferenceSetType;
        private readonly Type _packageReferenceSetListType;
        private readonly MethodInfo _listAdd;
        private readonly Type _packageReferenceSetEnumerableType;
        private readonly MethodInfo _getFirst;

        public FrameworkApi2x(AssemblyName assemblyName)
        {
            var assembly = assemblyName.GetAssembly();

            // PackageReferenceSet
            _packageReferenceSetType = assembly.GetType("NuGet.PackageReferenceSet");

            // List<PackageReferenceSet>
            var listType = typeof(List<>);
            _packageReferenceSetListType = listType
                .MakeGenericType(_packageReferenceSetType);

            _listAdd = _packageReferenceSetListType
                .GetMethod("Add");

            // IEnumerable<PackageReferenceSet>
            var ienumerableType = typeof(IEnumerable<>);
            _packageReferenceSetEnumerableType = ienumerableType
                .MakeGenericType(_packageReferenceSetType);

            // Enumerable
            var enumerableType = typeof(Enumerable);
            _getFirst = enumerableType
                .GetMethods()
                .First(x => x.Name == "First" && x.GetParameters().Length == 1)
                .MakeGenericMethod(_packageReferenceSetType);

            // VersionUtility
            var versionUtilityType = assembly.GetType("NuGet.VersionUtility");

            _getShortFolderName = versionUtilityType
                .GetMethod("GetShortFrameworkName");

            _parse = versionUtilityType
                .GetMethod("ParseFrameworkName");

            _isCompatible = versionUtilityType
                .GetMethod("IsCompatible");

            _tryGetCompatibleItems = versionUtilityType
                .GetMethod("TryGetCompatibleItems")
                .MakeGenericMethod(_packageReferenceSetType);
        }

        public string GetDotNetFrameworkName(object frameworkName)
        {
            return ((FrameworkName)frameworkName).ToString();
        }

        public string GetShortFolderName(object frameworkName)
        {
            return (string)_getShortFolderName.Invoke(null, new[] { frameworkName });
        }

        public string GetIdentifer(object frameworkName)
        {
            return ((FrameworkName)frameworkName).Identifier;
        }

        public System.Version GetVersion(object frameworkName)
        {
            return ((FrameworkName)frameworkName).Version;
        }

        public string GetProfile(object frameworkName)
        {
            return ((FrameworkName)frameworkName).Profile;
        }

        public object Parse(string input)
        {
            if (input.Contains(','))
            {
                return new FrameworkName(input);
            }

            return _parse.Invoke(null, new[] { input });
        }

        public object GetNearest(object project, IEnumerable<object> package)
        {
            var prsList = Activator.CreateInstance(_packageReferenceSetListType);
            var prsToFramework = new Dictionary<object, object>();
            foreach (var o in package)
            {
                var prs = Activator.CreateInstance(
                    _packageReferenceSetType,
                    o,
                    Enumerable.Empty<string>());

                _listAdd.Invoke(prsList, new[] { prs });
                prsToFramework[prs] = o;
            }

            var args = new[] { project, prsList, null };
            var matched = (bool)_tryGetCompatibleItems.Invoke(null, args);

            if (!matched)
            {
                return null;
            }

            var first = _getFirst.Invoke(null, new[] { args[2] });
            return prsToFramework[first];
        }

        public bool IsCompatible(object project, object package)
        {
            return (bool)_isCompatible.Invoke(
                null,
                new[]
                {
                    project,
                    new List<FrameworkName>
                    {
                        (FrameworkName) package
                    }
                });
        }

        public bool HasProfile(object nuGetFramework)
        {
            return !string.IsNullOrEmpty(GetProfile(nuGetFramework));
        }

        public bool HasPlatform(object nuGetFramework)
        {
            return false;
        }

        public string GetPlatform(object nuGetFramework)
        {
            throw new NotSupportedException();
        }

        public System.Version GetPlatformVersion(object nuGetFramework)
        {
            throw new NotSupportedException();
        }

        public string GetToStringResult(object nuGetFramework)
        {
            return ((FrameworkName)nuGetFramework).ToString();
        }
    }
}
