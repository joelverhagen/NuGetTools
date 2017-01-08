using System;
using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class FrameworkList<TFramework> : IFrameworkList where TFramework : IFramework
    {
        private readonly IFrameworkEnumerator<TFramework> _enumerator;
        private readonly Lazy<IReadOnlyList<FrameworkListItem>> _items;
        private readonly Lazy<IReadOnlyList<string>> _dotNetFrameworkNames;
        private readonly Lazy<IReadOnlyList<string>> _shortFolderNames;
        private readonly Lazy<IReadOnlyList<string>> _identifiers;

        public FrameworkList(IFrameworkEnumerator<TFramework> enumerator)
        {
            _enumerator = enumerator;
            _items = new Lazy<IReadOnlyList<FrameworkListItem>>(GetItems);
            _dotNetFrameworkNames = new Lazy<IReadOnlyList<string>>(GetDotNetFrameworkNames);
            _shortFolderNames = new Lazy<IReadOnlyList<string>>(GetShortFolderNames);
            _identifiers = new Lazy<IReadOnlyList<string>>(GetIdentifiers);
        }
        
        public IReadOnlyList<string> DotNetFrameworkNames => _dotNetFrameworkNames.Value;

        public IReadOnlyList<string> ShortFolderNames => _shortFolderNames.Value;

        public IReadOnlyList<string> Identifiers => _identifiers.Value;

        private IReadOnlyList<string> GetDotNetFrameworkNames()
        {
            return _items
                .Value
                .Select(x => x.DotNetFrameworkName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private IReadOnlyList<string> GetShortFolderNames()
        {
            return _items
                .Value
                .Select(x => x.ShortFolderName)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private IReadOnlyList<string> GetIdentifiers()
        {
            return _items
                .Value
                .Select(x => x.Identifier)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private IReadOnlyList<FrameworkListItem> GetItems()
        {
            return EnumerateItems().ToList();
        }

        private IEnumerable<FrameworkListItem> EnumerateItems()
        {
            var enumerated = _enumerator.Enumerate(FrameworkEnumerationOptions.All);
            var expanded = _enumerator.Expand(enumerated, FrameworkExpansionOptions.All);

            foreach (var framework in expanded)
            {
                yield return new FrameworkListItem(
                    framework.Identifier,
                    framework.Version,
                    framework.Profile,
                    framework.Framework.DotNetFrameworkName,
                    framework.Framework.ShortFolderName);
            }
        }
    }
}
