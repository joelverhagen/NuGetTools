using System;
using System.Collections.Generic;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class VersionRangeApi2x : IVersionRangeApi
    {
        private readonly MethodInfo _parse;
        private readonly MethodInfo _getMinVersion;
        private readonly MethodInfo _isMinInclusive;
        private readonly MethodInfo _getMaxVersion;
        private readonly MethodInfo _isMaxInclusive;
        private readonly MethodInfo _satisfies;
        private readonly MethodInfo _getNormalizedString;
        private readonly MethodInfo _prettyPrint;

        public VersionRangeApi2x(AssemblyName assemblyName)
        {
            var assembly = assemblyName.GetAssembly();

            // VersionUtility
            var versionUtilityType = assembly.GetType("NuGet.VersionUtility");

            _parse = versionUtilityType
                .GetMethod("ParseVersionSpec");

            _prettyPrint = versionUtilityType
                .GetMethod("PrettyPrint");

            // VersionExtensions
            var versionExtensionsType = assembly.GetType("NuGet.VersionExtensions");

            _satisfies = versionExtensionsType
                .GetMethod("Satisfies");

            // VersionSpec
            var versionSpecType = assembly.GetType("NuGet.VersionSpec");

            _getMinVersion = versionSpecType
                .GetProperty("MinVersion")
                .GetGetMethod();

            _isMinInclusive = versionSpecType
                .GetProperty("IsMinInclusive")
                .GetGetMethod();

            _getMaxVersion = versionSpecType
                .GetProperty("MaxVersion")
                .GetGetMethod();

            _isMaxInclusive = versionSpecType
                .GetProperty("IsMaxInclusive")
                .GetGetMethod();

            _getNormalizedString = versionSpecType
                .GetMethod("ToString");
        }

        public object Parse(string input)
        {
            return _parse.Invoke(null, new[] { input });
        }

        public object FindBestMatch(object versionSpec, IEnumerable<object> versions)
        {
            throw new NotSupportedException();
        }

        public bool HasLowerBound(object versionSpec)
        {
            return GetMinVersion(versionSpec) != null;
        }

        public bool HasUpperBound(object versionSpec)
        {
            return GetMaxVersion(versionSpec) != null;
        }

        public bool IsMaxInclusive(object versionSpec)
        {
            return (bool)_isMaxInclusive.Invoke(versionSpec, new object[0]);
        }

        public object GetMaxVersion(object versionSpec)
        {
            return _getMaxVersion.Invoke(versionSpec, new object[0]);
        }

        public string GetNormalizedString(object versionSpec)
        {
            return (string)_getNormalizedString.Invoke(versionSpec, new object[0]);
        }

        public string PrettyPrint(object versionSpec)
        {
            return (string)_prettyPrint.Invoke(null, new[] { versionSpec });
        }

        public object GetMinVersion(object versionSpec)
        {
            return _getMinVersion.Invoke(versionSpec, new object[0]);
        }

        public bool IsMinInclusive(object versionSpec)
        {
            return (bool)_isMinInclusive.Invoke(versionSpec, new object[0]);
        }

        public bool IsFloating(object nuGetVersionRange)
        {
            throw new NotSupportedException();
        }

        public bool IsFloatingAvailable()
        {
            return false;
        }

        public bool Satisfies(object versionSpec, object semanticVersion)
        {
            return (bool)_satisfies.Invoke(null, new[] { versionSpec, semanticVersion });
        }

        public string GetOriginalString(object nuGetVersionRange)
        {
            throw new NotSupportedException();
        }

        public bool GetOriginalStringAvailable()
        {
            return false;
        }

        public string GetLegacyString(object nuGetVersionRange)
        {
            throw new NotSupportedException();
        }

        public bool GetLegacyStringAvailable()
        {
            return false;
        }

        public string GetLegacyShortString(object nuGetVersionRange)
        {
            throw new NotSupportedException();
        }

        public bool GetLegacyShortStringAvailable()
        {
            return false;
        }

        public bool FindBestMatchAvailable()
        {
            return false;
        }
    }
}
