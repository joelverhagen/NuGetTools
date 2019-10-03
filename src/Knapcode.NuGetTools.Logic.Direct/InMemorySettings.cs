using System;
using System.Collections.Generic;
using NuGet.Configuration;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class InMemorySettings : ISettings
    {
        private string _globalPackagesFolder;
        private readonly Dictionary<string, InMemorySettingSection> _settings = new Dictionary<string, InMemorySettingSection>();

        public string FileName => throw new NotImplementedException();
        public IEnumerable<ISettings> Priority => throw new NotImplementedException();
        public string Root => throw new NotImplementedException();

        public event EventHandler SettingsChanged;

        public bool DeleteSection(string section) => throw new NotImplementedException();
        public bool DeleteValue(string section, string key) => throw new NotImplementedException();
        public IList<KeyValuePair<string, string>> GetNestedValues(string section, string subSection) => throw new NotImplementedException();
        public IList<SettingValue> GetSettingValues(string section, bool isPath = false) => throw new NotImplementedException();
        public string GetValue(string section, string key, bool isPath = false) => throw new NotImplementedException();
        public void SetNestedValues(string section, string subSection, IList<KeyValuePair<string, string>> values) => throw new NotImplementedException();
        public void SetValue(string section, string key, string value) => throw new NotImplementedException();
        public void SetValues(string section, IReadOnlyList<SettingValue> values) => throw new NotImplementedException();
        public void UpdateSections(string section, IReadOnlyList<SettingValue> values) => throw new NotImplementedException();
        public void Remove(string sectionName, SettingItem item) => throw new NotImplementedException();
        public IList<string> GetConfigFilePaths() => throw new NotImplementedException();
        public IList<string> GetConfigRoots() => throw new NotImplementedException();

        public void AddOrUpdate(string sectionName, SettingItem item)
        {
            var section = GetInMemorySettingSection(sectionName);
            section.AddItem(item);
        }

        public SettingSection GetSection(string sectionName)
        {
            return GetInMemorySettingSection(sectionName);
        }

        private InMemorySettingSection GetInMemorySettingSection(string sectionName)
        {
            if (!_settings.TryGetValue(sectionName, out var section))
            {
                section = new InMemorySettingSection(
                    sectionName,
                    new Dictionary<string, string>(),
                    new List<SettingItem>());
                _settings.Add(sectionName, section);
            }

            return section;
        }

        public void SaveToDisk()
        {
        }
    }
}
