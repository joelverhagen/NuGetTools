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

        public FrameworkList(IFrameworkEnumerator<TFramework> enumerator)
        {
            _enumerator = enumerator;
            _items = new Lazy<IReadOnlyList<FrameworkListItem>>(GetItems);
            _dotNetFrameworkNames = new Lazy<IReadOnlyList<string>>(GetDotNetFrameworkNames);
        }
        
        public IReadOnlyList<string> DotNetFrameworkNames => _dotNetFrameworkNames.Value;

        private IReadOnlyList<string> GetDotNetFrameworkNames()
        {
            return _items
                .Value
                .Select(x => x.DotNetFrameworkName)
                .Distinct()
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
