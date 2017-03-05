using System;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
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
        private readonly bool _isSemVer2Available;
        private readonly bool _getFullStringAvailable;

        public VersionApi(Assembly assembly)
        {
            Assembly = assembly;

            // SemanticVersion
            var semanticVersionType = Assembly.GetType("NuGet.Versioning.SemanticVersion");

            _isPrerelease = semanticVersionType
                .GetProperty("IsPrerelease")
                .GetGetMethod();

            _getNormalizedString = semanticVersionType
                .GetMethod("ToNormalizedString");

            _compare = semanticVersionType
                .GetMethod("CompareTo", new[] { semanticVersionType });

            // NuGetVersion
            var nuGetVersionType = Assembly.GetType("NuGet.Versioning.NuGetVersion");

            _getRevision = nuGetVersionType
                .GetProperty("Revision")
                .GetGetMethod();

            _parse = nuGetVersionType
                .GetMethod("Parse", new[] { typeof(string) });

            // NuGetVersion (3.4.3+)
            _isSemVer2 = nuGetVersionType
                .GetProperty("IsSemVer2")?
                .GetGetMethod();
            _isSemVer2Available = _isSemVer2 != null;

            // SemanticVersion (3.5.0-beta-final+)
            _getFullString = semanticVersionType
                .GetMethod("ToFullString");
            _getFullStringAvailable = _getFullString != null;
        }

        public VersionApi(AssemblyName assemblyName) : this(assemblyName.GetAssembly())
        {
        }

        public Assembly Assembly { get; }

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
            if (!_getFullStringAvailable)
            {
                throw new NotSupportedException();
            }

            return (string)_getFullString.Invoke(nuGetVersion, new object[0]);
        }

        public bool GetFullStringAvailable()
        {
            return _getFullStringAvailable;
        }

        public bool IsSemVer2(object nuGetVersion)
        {
            if (!_isSemVer2Available)
            {
                throw new NotSupportedException();
            }

            return (bool)_isSemVer2.Invoke(nuGetVersion, new object[0]);
        }

        public bool IsSemVer2Available()
        {
            return _isSemVer2Available;
        }
    }
}
