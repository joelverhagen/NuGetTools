using System;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection
{
    public class VersionApi
    {
        private readonly MethodInfo _parse;
        private readonly MethodInfo _getRevision;
        private readonly MethodInfo _isPrerelease;
        private readonly MethodInfo _isSemVer2;
        private readonly MethodInfo _getFullString;
        private readonly MethodInfo _getNormalizedString;
        private readonly MethodInfo _compare;

        public VersionApi(AssemblyName assemblyName)
        {
            // SemanticVersion
            var semanticTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Versioning.SemanticVersion");

            var semanticVersionType = Type.GetType(semanticTypeName);

            _isPrerelease = semanticVersionType
                .GetProperty("IsPrerelease")
                .GetGetMethod();

            _getFullString = semanticVersionType
                .GetMethod("ToFullString");

            _getNormalizedString = semanticVersionType
                .GetMethod("ToNormalizedString");

            _compare = semanticVersionType
                .GetMethod("CompareTo", new[] { semanticVersionType });

            // NuGetVersion
            var nuGetVersionTypeName = Assembly.CreateQualifiedName(
                assemblyName.FullName,
                "NuGet.Versioning.NuGetVersion");

            var nuGetVersionType = Type.GetType(nuGetVersionTypeName);

            _getRevision = nuGetVersionType
                .GetProperty("Revision")
                .GetGetMethod();

            _isSemVer2 = nuGetVersionType
                .GetProperty("IsSemVer2")
                .GetGetMethod();

            _parse = nuGetVersionType
                .GetMethod("Parse", new[] { typeof(string) });
        }
        
        public object Parse(string input)
        {
            return _parse.Invoke(null, new[] { input });
        }

        public int Compare(object nuGetVersionA, object nuGetVersionB)
        {
            return (int)_compare.Invoke(nuGetVersionA, new[] { nuGetVersionB });
        }

        public int GetRevision(object nuGetVersion)
        {
            return (int)_getRevision.Invoke(nuGetVersion, new object[0]);
        }

        public bool IsPrerelease(object nuGetVersion)
        {
            return (bool)_isPrerelease.Invoke(nuGetVersion, new object[0]);
        }

        public string GetNormalizedString(object nuGetVersion)
        {
            return (string)_getNormalizedString.Invoke(nuGetVersion, new object[0]);
        }

        public string GetFullString(object nuGetVersion)
        {
            return (string)_getFullString.Invoke(nuGetVersion, new object[0]);
        }

        public bool IsSemVer2(object nuGetVersion)
        {
            return (bool)_isSemVer2.Invoke(nuGetVersion, new object[0]);
        }
    }
}
