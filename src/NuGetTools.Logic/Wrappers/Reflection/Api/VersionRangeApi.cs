using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public class VersionRangeApi
    {
        private readonly MethodInfo _parse;
        private readonly Type _nuGetVersionListType;
        private readonly MethodInfo _listAdd;
        private readonly MethodInfo _hasLowerBound;
        private readonly MethodInfo _hasUpperBound;
        private readonly MethodInfo _isFloating;
        private readonly MethodInfo _isMinInclusive;
        private readonly MethodInfo _isMaxInclusive;
        private readonly MethodInfo _getMinVersion;
        private readonly MethodInfo _getMaxVersion;
        private readonly MethodInfo _getNormalizedString;
        private readonly MethodInfo _prettyPrint;
        private readonly MethodInfo _satisfies;
        private readonly MethodInfo _findBestMatch;

        public VersionRangeApi(AssemblyName assemblyName)
        {
            var assembly = assemblyName.GetAssembly();

            // NuGetVersion
            var nuGetVersionType = assembly.GetType("NuGet.Versioning.NuGetVersion");

            // VersionRangeBase
            var versionRangeBaseType = assembly.GetType("NuGet.Versioning.VersionRangeBase");

            _hasLowerBound = versionRangeBaseType
                .GetProperty("HasLowerBound")
                .GetGetMethod();

            _hasUpperBound = versionRangeBaseType
                .GetProperty("HasUpperBound")
                .GetGetMethod();

            _isMinInclusive = versionRangeBaseType
                .GetProperty("IsMinInclusive")
                .GetGetMethod();

            _isMaxInclusive = versionRangeBaseType
                .GetProperty("IsMaxInclusive")
                .GetGetMethod();

            _getMinVersion = versionRangeBaseType
                .GetProperty("MinVersion")
                .GetGetMethod();

            _getMaxVersion = versionRangeBaseType
                .GetProperty("MaxVersion")
                .GetGetMethod();

            _satisfies = versionRangeBaseType
                .GetMethod("Satisfies", new[] { nuGetVersionType });

            // VersionRange
            var nuGetVersionRangeType = assembly.GetType("NuGet.Versioning.VersionRange");

            _getNormalizedString = nuGetVersionRangeType
                .GetMethod("ToNormalizedString");

            _isFloating = nuGetVersionRangeType
                .GetProperty("IsFloating")
                .GetGetMethod();

            _prettyPrint = nuGetVersionRangeType
                .GetMethod("PrettyPrint");

            _parse = nuGetVersionRangeType
                .GetMethod("Parse", new[] { typeof(string) });

            _findBestMatch = nuGetVersionRangeType
                .GetMethod("FindBestMatch");

            // List<NuGetVersion>
            var listType = typeof(List<>);
            _nuGetVersionListType = listType.MakeGenericType(nuGetVersionType);

            _listAdd = _nuGetVersionListType
                .GetMethod("Add");
        }

        public object Parse(string input)
        {
            return _parse.Invoke(null, new[] { input });
        }

        public object FindBestMatch(object versionRange, IEnumerable<object> versions)
        {
            var versionList = Activator.CreateInstance(_nuGetVersionListType);
            foreach (var o in versions)
            {
                _listAdd.Invoke(versionList, new[] { o });
            }

            return _findBestMatch.Invoke(versionRange, new[] { versionList });
        }

        public bool HasLowerBound(object nuGetVersionRange)
        {
            return (bool)_hasLowerBound.Invoke(nuGetVersionRange, new object[0]);
        }

        public bool HasUpperBound(object nuGetVersionRange)
        {
            return (bool)_hasUpperBound.Invoke(nuGetVersionRange, new object[0]);
        }

        public bool IsMaxInclusive(object nuGetVersionRange)
        {
            return (bool)_isMaxInclusive.Invoke(nuGetVersionRange, new object[0]);
        }

        public object GetMaxVersion(object nuGetVersionRange)
        {
            return _getMaxVersion.Invoke(nuGetVersionRange, new object[0]);
        }

        public string GetNormalizedString(object nuGetVersionRange)
        {
            return (string)_getNormalizedString.Invoke(nuGetVersionRange, new object[0]);
        }

        public string PrettyPrint(object nuGetVersionRange)
        {
            return (string)_prettyPrint.Invoke(nuGetVersionRange, new object[0]);
        }

        public object GetMinVersion(object nuGetVersionRange)
        {
            return _getMinVersion.Invoke(nuGetVersionRange, new object[0]);
        }

        public bool IsMinInclusive(object nuGetVersionRange)
        {
            return (bool)_isMinInclusive.Invoke(nuGetVersionRange, new object[0]);
        }

        public bool IsFloating(object nuGetVersionRange)
        {
            return (bool)_isFloating.Invoke(nuGetVersionRange, new object[0]);
        }

        public bool Satisfies(object nuGetVersionRange, object nuGetVersion)
        {
            return (bool)_satisfies.Invoke(nuGetVersionRange, new[] { nuGetVersion });
        }
    }
}
