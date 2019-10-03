using System;
using System.Collections.Generic;
using NuGet.Configuration;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class InMemorySettingSection : SettingSection
    {
        public readonly List<SettingItem> _items = new List<SettingItem>();

        public InMemorySettingSection(
            string name,
            IReadOnlyDictionary<string, string> attributes,
            IEnumerable<SettingItem> children) : base(name, attributes, children)
        {
        }

        public void AddItem(SettingItem item) => Children.Add(item);

        public override SettingBase Clone() => throw new NotImplementedException();
    }
}
