using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Knapcode.NuGetTools.Logic.Direct.Wrappers;
using NuGet.Frameworks;
using FrameworkEnumeratorData = Knapcode.NuGetTools.Logic.FrameworkEnumeratorData<Knapcode.NuGetTools.Logic.Direct.Wrappers.Framework>;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class FrameworkEnumerator : IFrameworkEnumerator<Framework>
    {
        public IEnumerable<FrameworkEnumeratorData> Expand(IEnumerable<FrameworkEnumeratorData> frameworks, FrameworkExpansionOptions options)
        {
            var existing = new HashSet<FrameworkEnumeratorData>();

            foreach (var data in frameworks)
            {
                var originalAdded = AddFramework(existing, data);
                if (originalAdded != null)
                {
                    yield return originalAdded;
                }

                if (options.HasFlag(FrameworkExpansionOptions.RoundTripDotNetFrameworkName))
                {
                    foreach (var added in ExpandByRoundTrippingDotNetFrameworkName(existing, data))
                    {
                        yield return added;
                    }
                }

                if (options.HasFlag(FrameworkExpansionOptions.RoundTripShortFolderName))
                {
                    foreach (var added in ExpandByRoundTrippingShortFolderName(existing, data))
                    {
                        yield return added;
                    }
                }

                if (options.HasFlag(FrameworkExpansionOptions.FrameworkExpander))
                {
                    var expander = new FrameworkExpander();

                    foreach (var added in ExpandByUsingFrameworkExpander(existing, data, expander))
                    {
                        yield return added;
                    }
                }
            }
        }

        public IEnumerable<FrameworkEnumeratorData> Enumerate(FrameworkEnumerationOptions options)
        {
            var existing = new HashSet<FrameworkEnumeratorData>();
            
            if (options.HasFlag(FrameworkEnumerationOptions.FrameworkNameProvider))
            {
                foreach (var added in AddDefaultFrameworkNameProvider(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumerationOptions.CommonFrameworks))
            {
                foreach (var added in AddCommonFrameworks(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumerationOptions.FrameworkMappings))
            {
                foreach (var added in AddDefaultFrameworkMappings(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumerationOptions.PortableFrameworkMappings))
            {
                foreach (var added in AddDefaultPortableFrameworkMappings(existing))
                {
                    yield return added;
                }
            }

            if (options.HasFlag(FrameworkEnumerationOptions.SpecialFrameworks))
            {
                foreach (var added in AddSpecialFrameworks(existing))
                {
                    yield return added;
                }
            }
        }

        private IEnumerable<FrameworkEnumeratorData> AddSpecialFrameworks(HashSet<FrameworkEnumeratorData> existing)
        {
            var specialFrameworks = new[]
                {
                    NuGetFramework.AgnosticFramework,
                    NuGetFramework.AnyFramework,
                    NuGetFramework.UnsupportedFramework
                }
                .Select(GetFrameworkEnumeratorData);

            return AddFrameworks(existing, specialFrameworks);
        }

        private static IEnumerable<FrameworkEnumeratorData> AddDefaultFrameworkNameProvider(HashSet<FrameworkEnumeratorData> existing)
        {
            var frameworkNameProvider = DefaultFrameworkNameProvider.Instance;

            var compatibilityCandidates = frameworkNameProvider
                .GetCompatibleCandidates()
                .Select(GetFrameworkEnumeratorData);

            return AddFrameworks(existing, compatibilityCandidates);
        }

        private static IEnumerable<FrameworkEnumeratorData> AddDefaultPortableFrameworkMappings(HashSet<FrameworkEnumeratorData> existing)
        {
            var portableFrameworkMappings = DefaultPortableFrameworkMappings.Instance;

            var profileFrameworks = portableFrameworkMappings
                .ProfileFrameworks
                .SelectMany(x => x.Value)
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, profileFrameworks))
            {
                yield return added;
            }

            var profileFrameworksNumbers = portableFrameworkMappings
                .ProfileFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, profileFrameworksNumbers))
            {
                yield return added;
            }

            var profileOptionalFrameworks = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .SelectMany(x => x.Value)
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworks))
            {
                yield return added;
            }

            var profileOptionalFrameworksNumbers = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworksNumbers))
            {
                yield return added;
            }

            var compatibilityMappings = portableFrameworkMappings
                .CompatibilityMappings
                .SelectMany(x => new[] { x.Value.Min, x.Value.Max })
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var compatibilityMappingsNumbers = portableFrameworkMappings
                .CompatibilityMappings
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, compatibilityMappingsNumbers))
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkEnumeratorData> AddDefaultFrameworkMappings(HashSet<FrameworkEnumeratorData> existing)
        {
            var frameworkMappings = DefaultFrameworkMappings.Instance;

            var equivalentFrameworks = frameworkMappings
                .EquivalentFrameworks
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, equivalentFrameworks))
            {
                yield return added;
            }

            var compatibilityMappings = frameworkMappings
                .CompatibilityMappings
                .SelectMany(x => (new[]
                {
                        x.SupportedFrameworkRange.Min,
                        x.SupportedFrameworkRange.Max,
                        x.TargetFrameworkRange.Min,
                        x.TargetFrameworkRange.Max
                }))
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var shortNameReplacements = frameworkMappings
                .ShortNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, shortNameReplacements))
            {
                yield return added;
            }

            var fullNameReplacements = frameworkMappings
                .FullNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkEnumeratorData);

            foreach (var added in AddFrameworks(existing, fullNameReplacements))
            {
                yield return added;
            }
        }

        private static FrameworkEnumeratorData<Framework> GetFrameworkEnumeratorData(NuGetFramework x)
        {
            return new FrameworkEnumeratorData(new Framework(x));
        }

        private static IEnumerable<FrameworkEnumeratorData> AddCommonFrameworks(HashSet<FrameworkEnumeratorData> existing)
        {
            var commonFrameworksType = typeof(FrameworkConstants.CommonFrameworks);

            var commonFrameworks = commonFrameworksType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(NuGetFramework))
                .Select(x => x.GetValue(null))
                .Cast<NuGetFramework>()
                .Select(GetFrameworkEnumeratorData);

            return AddFrameworks(existing, commonFrameworks);
        }

        private static IEnumerable<FrameworkEnumeratorData> ExpandByRoundTrippingShortFolderName(
            HashSet<FrameworkEnumeratorData> existing,
            FrameworkEnumeratorData data)
        {
            var shortFolderName = data.Framework.NuGetFramework.GetShortFolderName();
            var roundTrip = GetFrameworkEnumeratorData(NuGetFramework.ParseFolder(shortFolderName));

            var added = AddFramework(existing, roundTrip);
            if (added != null)
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkEnumeratorData> ExpandByRoundTrippingDotNetFrameworkName(
            HashSet<FrameworkEnumeratorData> existing,
            FrameworkEnumeratorData data)
        {
            var dotNetFrameworkName = data.Framework.NuGetFramework.DotNetFrameworkName;
            var roundTrip = GetFrameworkEnumeratorData(NuGetFramework.Parse(dotNetFrameworkName));

            var added = AddFramework(existing, roundTrip);
            if (added != null)
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkEnumeratorData> ExpandByUsingFrameworkExpander(
            HashSet<FrameworkEnumeratorData> existing,
            FrameworkEnumeratorData data,
            FrameworkExpander expander)
        {
            var expanded = expander
                .Expand(data.Framework.NuGetFramework)
                .Select(GetFrameworkEnumeratorData);

            if (expanded.Any())
            {
                foreach (var added in AddFrameworks(existing, expanded))
                {
                    yield return added;
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

        private static IEnumerable<FrameworkEnumeratorData> AddFrameworks(HashSet<FrameworkEnumeratorData> existing, IEnumerable<FrameworkEnumeratorData> toAdd)
        {
            foreach (var data in toAdd)
            {
                var added = AddFramework(existing, data);
                if (added != null)
                {
                    yield return added;
                }
            }
        }

        private static FrameworkEnumeratorData AddFramework(HashSet<FrameworkEnumeratorData> existing, FrameworkEnumeratorData data)
        {
            if (data.Version == FrameworkConstants.MaxVersion)
            {
                return null;
            }

            if (!existing.Add(data))
            {
                return null;
            }

            return data;
        }
    }
}
