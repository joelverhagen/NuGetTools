using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Knapcode.NuGetTools.Logic.Direct.Wrappers;
using NuGet.Frameworks;
using FrameworkData = Knapcode.NuGetTools.Logic.FrameworkData<Knapcode.NuGetTools.Logic.Direct.Wrappers.Framework>;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class FrameworkEnumerator : IFrameworkEnumerator<Framework>
    {
        public IEnumerable<FrameworkData> Expand(IEnumerable<FrameworkData> frameworks, FrameworkExpansionOptions options)
        {
            var existing = new HashSet<FrameworkData>();

            foreach (var frameworkData in frameworks)
            {
                var originalAdded = AddFramework(existing, frameworkData);
                if (originalAdded != null)
                {
                    yield return originalAdded;
                }

                if (options.HasFlag(FrameworkExpansionOptions.RoundTripDotNetFrameworkName))
                {
                    foreach (var added in ExpandByRoundTrippingDotNetFrameworkName(existing, frameworkData))
                    {
                        yield return added;
                    }
                }

                if (options.HasFlag(FrameworkExpansionOptions.RoundTripShortFolderName))
                {
                    foreach (var added in ExpandByRoundTrippingShortFolderName(existing, frameworkData))
                    {
                        yield return added;
                    }
                }

                if (options.HasFlag(FrameworkExpansionOptions.FrameworkExpander))
                {
                    var expander = new FrameworkExpander();

                    foreach (var added in ExpandByUsingFrameworkExpander(existing, frameworkData, expander))
                    {
                        yield return added;
                    }
                }
            }
        }

        public IEnumerable<FrameworkData> Enumerate(FrameworkEnumerationOptions options)
        {
            var existing = new HashSet<FrameworkData>();
            
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

        private IEnumerable<FrameworkData> AddSpecialFrameworks(HashSet<FrameworkData> existing)
        {
            var specialFrameworks = new[]
                {
                    NuGetFramework.AgnosticFramework,
                    NuGetFramework.AnyFramework,
                    NuGetFramework.UnsupportedFramework
                }
                .Select(GetFrameworkData);

            return AddFrameworks(existing, specialFrameworks);
        }

        private static IEnumerable<FrameworkData> AddDefaultFrameworkNameProvider(HashSet<FrameworkData> existing)
        {
            var frameworkNameProvider = DefaultFrameworkNameProvider.Instance;

            var compatibilityCandidates = frameworkNameProvider
                .GetCompatibleCandidates()
                .Select(GetFrameworkData);

            return AddFrameworks(existing, compatibilityCandidates);
        }

        private static IEnumerable<FrameworkData> AddDefaultPortableFrameworkMappings(HashSet<FrameworkData> existing)
        {
            var portableFrameworkMappings = DefaultPortableFrameworkMappings.Instance;

            var profileFrameworks = portableFrameworkMappings
                .ProfileFrameworks
                .SelectMany(x => x.Value)
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, profileFrameworks))
            {
                yield return added;
            }

            var profileFrameworksNumbers = portableFrameworkMappings
                .ProfileFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, profileFrameworksNumbers))
            {
                yield return added;
            }

            var profileOptionalFrameworks = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .SelectMany(x => x.Value)
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworks))
            {
                yield return added;
            }

            var profileOptionalFrameworksNumbers = portableFrameworkMappings
                .ProfileOptionalFrameworks
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, profileOptionalFrameworksNumbers))
            {
                yield return added;
            }

            var compatibilityMappings = portableFrameworkMappings
                .CompatibilityMappings
                .SelectMany(x => new[] { x.Value.Min, x.Value.Max })
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var compatibilityMappingsNumbers = portableFrameworkMappings
                .CompatibilityMappings
                .Select(x => GetPortableFramework(x.Key))
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, compatibilityMappingsNumbers))
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkData> AddDefaultFrameworkMappings(HashSet<FrameworkData> existing)
        {
            var frameworkMappings = DefaultFrameworkMappings.Instance;

            var equivalentFrameworks = frameworkMappings
                .EquivalentFrameworks
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkData);

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
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, compatibilityMappings))
            {
                yield return added;
            }

            var shortNameReplacements = frameworkMappings
                .ShortNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, shortNameReplacements))
            {
                yield return added;
            }

            var fullNameReplacements = frameworkMappings
                .FullNameReplacements
                .SelectMany(x => new[] { x.Key, x.Value })
                .Select(GetFrameworkData);

            foreach (var added in AddFrameworks(existing, fullNameReplacements))
            {
                yield return added;
            }
        }

        private static FrameworkData<Framework> GetFrameworkData(NuGetFramework x)
        {
            return new FrameworkData(new Framework(x));
        }

        private static IEnumerable<FrameworkData> AddCommonFrameworks(HashSet<FrameworkData> existing)
        {
            var commonFrameworksType = typeof(FrameworkConstants.CommonFrameworks);

            var commonFrameworks = commonFrameworksType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(x => x.FieldType == typeof(NuGetFramework))
                .Select(x => x.GetValue(null))
                .Cast<NuGetFramework>()
                .Select(GetFrameworkData);

            return AddFrameworks(existing, commonFrameworks);
        }

        private static IEnumerable<FrameworkData> ExpandByRoundTrippingShortFolderName(HashSet<FrameworkData> existing, FrameworkData frameworkData)
        {
            var shortFolderName = frameworkData.Framework.NuGetFramework.GetShortFolderName();
            var roundTrip = GetFrameworkData(NuGetFramework.ParseFolder(shortFolderName));

            var added = AddFramework(existing, roundTrip);
            if (added != null)
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkData> ExpandByRoundTrippingDotNetFrameworkName(HashSet<FrameworkData> existing, FrameworkData frameworkData)
        {
            var dotNetFrameworkName = frameworkData.Framework.NuGetFramework.DotNetFrameworkName;
            var roundTrip = GetFrameworkData(NuGetFramework.Parse(dotNetFrameworkName));

            var added = AddFramework(existing, roundTrip);
            if (added != null)
            {
                yield return added;
            }
        }

        private static IEnumerable<FrameworkData> ExpandByUsingFrameworkExpander(HashSet<FrameworkData> existing, FrameworkData frameworkData, FrameworkExpander expander)
        {
            var expanded = expander
                .Expand(frameworkData.Framework.NuGetFramework)
                .Select(GetFrameworkData);

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

        private static IEnumerable<FrameworkData> AddFrameworks(HashSet<FrameworkData> existing, IEnumerable<FrameworkData> toAdd)
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

        private static FrameworkData AddFramework(HashSet<FrameworkData> existing, FrameworkData frameworkData)
        {
            if (frameworkData.Version == FrameworkConstants.MaxVersion)
            {
                return null;
            }

            if (!existing.Add(frameworkData))
            {
                return null;
            }

            return frameworkData;
        }
    }
}
