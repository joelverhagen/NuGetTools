using System;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class VersionApi2x : IVersionApi
    {
        private readonly MethodInfo _getSpecialVersion;
        private readonly MethodInfo _getNormalizedString;
        private readonly MethodInfo _compare;
        private readonly MethodInfo _getVersion;
        private readonly MethodInfo _parse;
        private readonly MethodInfo _isSemVer2;
        private readonly MethodInfo _getFullString;
        private readonly bool _getNormalizedStringAvailable;
        private readonly MethodInfo _getToString;
        private readonly bool _getFullStringAvailable;
        private readonly bool _isSemVer2Available;

        public VersionApi2x(AssemblyName assemblyName)
        {
            var assembly = assemblyName.GetAssembly();

            // SemanticVersion
            var semanticVersionType = assembly.GetType("NuGet.SemanticVersion");

            _getSpecialVersion = semanticVersionType
                .GetProperty("SpecialVersion")
                .GetGetMethod();

            _getNormalizedString = semanticVersionType
                .GetMethod("ToNormalizedString");
            _getNormalizedStringAvailable = _getNormalizedString != null;

            _compare = semanticVersionType
                .GetMethod("CompareTo", new[] { semanticVersionType });

            _getVersion = semanticVersionType
                .GetProperty("Version")
                .GetGetMethod();

            _parse = semanticVersionType
                .GetMethod("Parse");

            _isSemVer2 = semanticVersionType
                .GetMethod("IsSemVer2");
            _isSemVer2Available = _isSemVer2 != null;

            _getFullString = semanticVersionType
                .GetMethod("ToFullString");
            _getFullStringAvailable = _getFullString != null;

            _getToString = semanticVersionType
                .GetMethod("ToString", new Type[0]);
        }

        public object Parse(string input)
        {
            return _parse.Invoke(null, new[] { input });
        }

        public int Compare(object semanticVersionA, object semanticVersionB)
        {
            return (int)_compare.Invoke(semanticVersionA, new[] { semanticVersionB });
        }

        public int GetRevision(object semanticVersion)
        {
            var version = (System.Version)_getVersion.Invoke(semanticVersion, new object[0]);
            return version.Revision;
        }

        public bool IsPrerelease(object semanticVersion)
        {
            var specialVersion = (string)_getSpecialVersion.Invoke(semanticVersion, new object[0]);
            return !string.IsNullOrEmpty(specialVersion);
        }

        public string GetNormalizedString(object sematicVersion)
        {
            if (!_getNormalizedStringAvailable)
            {
                throw new NotSupportedException();
            }

            return (string)_getNormalizedString.Invoke(sematicVersion, new object[0]);
        }

        public bool GetNormalizedStringAvailable()
        {
            return _getNormalizedStringAvailable;
        }

        public string GetFullString(object semanticVersion)
        {
            if (!_getFullStringAvailable)
            {
                throw new NotSupportedException();
            }

            return (string)_getFullString.Invoke(semanticVersion, new object[0]);
        }

        public bool GetFullStringAvailable()
        {
            return _getFullStringAvailable;
        }

        public bool IsSemVer2(object semanticVersion)
        {
            if (!_isSemVer2Available)
            {
                throw new NotSupportedException();
            }

            return (bool)_isSemVer2.Invoke(semanticVersion, new object[0]);
        }

        public bool IsSemVer2Available()
        {
            return _isSemVer2Available;
        }

        public string GetToStringResult(object nuGetVersion)
        {
            return (string)_getToString.Invoke(nuGetVersion, new object[0]);
        }
    }
}
