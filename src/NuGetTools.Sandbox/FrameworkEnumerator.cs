using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Sandbox
{
    public class FrameworkEnumerator
    {
        public IEnumerable<string> Enumerate(FrameworkEnumeratorOptions options)
        {
            var existing = new HashSet<FrameworkData>();
            
            if (options.HasFlag(FrameworkEnumeratorOptions.FrameworkNameProvider))
            {
                foreach (var added in AddDefaultFrameworkNameProvider(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.CommonFrameworks))
            {
                foreach (var added in AddCommonFrameworks(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.FrameworkMappings))
            {
                foreach (var added in AddDefaultFrameworkMappings(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.PortableFrameworkMappings))
            {
                foreach (var added in AddDefaultPortableFrameworkMappings(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.SpecialFrameworks))
            {
                foreach (var added in AddSpecialFrameworks(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.RoundTripDotNetFrameworkName))
            {
                foreach (var added in ExpandByRoundTrippingDotNetFrameworkName(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.RoundTripShortFolderName))
            {
                foreach (var added in ExpandByRoundTrippingShortFolderName(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumeratorOptions.FrameworkExpander))
            {
                foreach (var added in ExpandByUsingFrameworkExpander(existing))
                {
                    yield return added;
                }
            }
        }

        private IEnumerable<string> AddSpecialFrameworks(HashSet<FrameworkData> existing)
        {
            var specialFrameworks = new[]
                {
                    NuGetFramework.AgnosticFramework,
                    NuGetFramework.AnyFramework,
                    NuGetFramework.UnsupportedFramework
                }
                .Select(x => new FrameworkData(x));

            return AddFrameworks(existing, specialFrameworks);
        }

        private static IEnumerable<string> AddDefaultFrameworkNameProvider(HashSet<FrameworkData> existing)
        {
            var frameworkNameProvider = DefaultFrameworkNameProvider.Instance;

            var compatibilityCandidates = frameworkNameProvider
                .GetCompatibleCandidates()
                .Select(x => new FrameworkData(x));

            return AddFrameworks(existing, compatibilityCandidates);
        }

        private static IEnumerable<string> AddDefaultPortableFrameworkMappings(HashSet<FrameworkData> existing)
        {
            var portableFrameworkMappings = DefaultPortableFrameworkMappings.Instance;

            var profileFrameworks = portableFrameworkMappings
                .ProfileFrameworks
                .SelectMany(x => x.Value)
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, profileFrameworks))
            {
                yield return added;
            }

            var profileFrameworksNumbers = portableFrameworkMappings
                .ProfileFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, profileFrameworksNumbers))
            {
                yield return added;
            }

            var profileOptionalFrameworks = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .SelectMany(x => x.Value)
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworks))
            {
                yield return added;
            }

            var profileOptionalFrameworksNumbers = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworksNumbers))
            {
                yield return added;
            }

            var compatibilityMappings = portableFrameworkMappings
                .CompatibilityMappings
                .SelectMany(x => new[] { x.Value.Min, x.Value.Max })
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var compatibilityMappingsNumbers = portableFrameworkMappings
                .CompatibilityMappings
                .Select(x => GetPortableFramework(x.Key))
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, compatibilityMappingsNumbers))
            {
                yield return added;
            }
        }

        private static IEnumerable<string> AddDefaultFrameworkMappings(HashSet<FrameworkData> existing)
        {
            var frameworkMappings = DefaultFrameworkMappings.Instance;

            var equivalentFrameworks = frameworkMappings
                .EquivalentFrameworks
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, equivalentFrameworks))
            {
                yield return added;
            }

            var compatibilityMappings = frameworkMappings
                .CompatibilityMappings
                .SelectMany(x => new[]
                {
                        x.SupportedFrameworkRange.Min,
                        x.SupportedFrameworkRange.Max,
                        x.TargetFrameworkRange.Min,
                        x.TargetFrameworkRange.Max
                })
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var shortNameReplacements = frameworkMappings
                .ShortNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, shortNameReplacements))
            {
                yield return added;
            }

            var fullNameReplacements = frameworkMappings
                .FullNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(x => new FrameworkData(x));

            foreach (var added in AddFrameworks(existing, fullNameReplacements))
            {
                yield return added;
            }
        }

        private static IEnumerable<string> AddCommonFrameworks(HashSet<FrameworkData> existing)
        {
            var commonFrameworksType = typeof(FrameworkConstants.CommonFrameworks);

            var commonFrameworks = commonFrameworksType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(NuGetFramework))
                .Select(x => x.GetValue(null))
                .Cast<NuGetFramework>()
                .Select(x => new FrameworkData(x));

            return AddFrameworks(existing, commonFrameworks);
        }

        private static IEnumerable<string> ExpandByRoundTrippingShortFolderName(HashSet<FrameworkData> existing)
        {
            foreach (var frameworkData in existing.ToList())
            {
                var shortFolderName = frameworkData.NuGetFramework.GetShortFolderName();
                var roundTrip = new FrameworkData(NuGetFramework.ParseFolder(shortFolderName));

                var added = AddFramework(existing, roundTrip);
                if (added != null)
                {
                    yield return added;
                }
            }
        }

        private static IEnumerable<string> ExpandByRoundTrippingDotNetFrameworkName(HashSet<FrameworkData> existing)
        {
            foreach (var frameworkData in existing.ToList())
            {
                var dotNetFrameworkName = frameworkData.NuGetFramework.DotNetFrameworkName;
                var roundTrip = new FrameworkData(NuGetFramework.Parse(dotNetFrameworkName));

                var added = AddFramework(existing, roundTrip);
                if (added != null)
                {
                    yield return added;
                }
            }
        }

        private static IEnumerable<string> ExpandByUsingFrameworkExpander(HashSet<FrameworkData> existing)
        {
            var frameworkExpander = new FrameworkExpander();
            foreach (var frameworkData in existing.ToList())
            {
                var expanded = frameworkExpander
                    .Expand(frameworkData.NuGetFramework)
                    .Select(x => new FrameworkData(x));

                if (expanded.Any())
                {
                    foreach (var added in AddFrameworks(existing, expanded))
                    {
                        yield return added;
                    }
                }
            }
        }

        private static NuGetFramework GetPortableFramework(int profileNumber)
        {
            return new NuGetFramework(
                FrameworkConstants.FrameworkIdentifiers.Portable,
                FrameworkConstants.EmptyVersion,
                FrameworkNameHelpers.GetPortableProfileNumberString(profileNumber));
        }

        private static IEnumerable<string> AddFrameworks(HashSet<FrameworkData> existing, IEnumerable<FrameworkData> toAdd)
        {
            foreach (var frameworkData in toAdd)
            {
                var added = AddFramework(existing, frameworkData);
                if (added != null)
                {
                    yield return added;
                }
            }
        }

        private static string AddFramework(HashSet<FrameworkData> existing, FrameworkData frameworkData)
        {
            if (frameworkData.Version == FrameworkConstants.MaxVersion)
            {
                return null;
            }

            if (!existing.Add(frameworkData))
            {
                return null;
            }

            return frameworkData.ToString();
        }

        private class FrameworkData : IEquatable<FrameworkData>, IComparable<FrameworkData>
        {
            public FrameworkData(NuGetFramework framework)
            {
                Framework = framework.Framework;
                Version = framework.Version;
                Profile = framework.Profile;
                NuGetFramework = new NuGetFramework(Framework, Version, Profile);
            }

            public string Framework { get; }
            public Version Version { get; }
            public string Profile { get; }
            public NuGetFramework NuGetFramework { get; }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Profile))
                {
                    return $"{Framework},Version=v{GetDisplayVersion(Version)}";
                }
                else
                {
                    return $"{Framework},Version=v{GetDisplayVersion(Version)},Profile={Profile}";
                }
            }

            public override bool Equals(object obj)
            {
                return Equals(obj as FrameworkData);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = 27;
                    hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Framework);
                    hash = (13 * hash) + Version.GetHashCode();
                    hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Profile);
                    return hash;
                }
            }

            public bool Equals(FrameworkData other)
            {
                if (other == null)
                {
                    return false;
                }

                return StringComparer.OrdinalIgnoreCase.Equals(Framework, other.Framework) &&
                       Version == other.Version &&
                       StringComparer.OrdinalIgnoreCase.Equals(Profile, other.Profile);
            }

            public int CompareTo(FrameworkData other)
            {
                var frameworkCompare = StringComparer.OrdinalIgnoreCase.Compare(Framework, other.Framework);
                if (frameworkCompare != 0)
                {
                    return frameworkCompare;
                }

                var versionCompare = Version.CompareTo(other.Version);
                if (versionCompare != 0)
                {
                    return frameworkCompare;
                }

                return Profile.CompareTo(other.Profile);
            }

            private static string GetDisplayVersion(Version version)
            {
                var sb = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor));

                if (version.Build > 0
                    || version.Revision > 0)
                {
                    sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}", version.Build);

                    if (version.Revision > 0)
                    {
                        sb.AppendFormat(CultureInfo.InvariantCulture, ".{0}", version.Revision);
                    }
                }

                return sb.ToString();
            }
        }
    }
}
