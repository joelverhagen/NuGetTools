using System;
using System.Collections.Generic;
using System.IO;
using NuGet.Configuration;

namespace Knapcode.NuGetTools.Logic
{
    public class InMemorySettings : ISettings
    {
        private readonly Dictionary<string, Dictionary<string, string>> _sections
            = new Dictionary<string, Dictionary<string, string>>();

        public string FileName
        {
            get
            {
                return Settings.DefaultSettingsFileName;
            }
        }

        public IEnumerable<ISettings> Priority
        {
            get
            {
                yield return this;
            }
        }

        public string Root
        {
            get
            {
                return ".";
            }
        }

        public event EventHandler SettingsChanged;

        public bool DeleteSection(string section)
        {
            return _sections.Remove(section);
        }

        public bool DeleteValue(string section, string key)
        {
            Dictionary<string, string> values;
            if (!_sections.TryGetValue(section, out values))
            {
                return false;
            }

            return values.Remove(key);
        }

        public IList<KeyValuePair<string, string>> GetNestedValues(string section, string subSection)
        {
            throw new NotImplementedException();
        }

        public IList<SettingValue> GetSettingValues(string section, bool isPath = false)
        {
            throw new NotImplementedException();
        }

        public string GetValue(string section, string key, bool isPath = false)
        {
            Dictionary<string, string> values;
            if (!_sections.TryGetValue(section, out values))
            {
                return null;
            }

            string value;
            if (!values.TryGetValue(key, out value))
            {
                return null;
            }

            if (!isPath)
            {
                return value;
            }

            return Path.GetFullPath(value);
        }

        public void SetNestedValues(string section, string subSection, IList<KeyValuePair<string, string>> values)
        {
            throw new NotImplementedException();
        }

        public void SetValue(string section, string key, string value)
        {
            Dictionary<string, string> values;
            if (!_sections.TryGetValue(section, out values))
            {
                values = new Dictionary<string, string>();
                _sections[section] = values;
            }

            values[key] = value;
        }

        public void SetValues(string section, IReadOnlyList<SettingValue> values)
        {
            throw new NotImplementedException();
        }

        public void UpdateSections(string section, IReadOnlyList<SettingValue> values)
        {
            throw new NotImplementedException();
        }
    }
}
