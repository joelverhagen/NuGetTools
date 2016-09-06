using System;
using System.Globalization;
using System.Text;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class FrameworkData<TFramework> : IEquatable<FrameworkData<TFramework>>, IComparable<FrameworkData<TFramework>>
        where TFramework : IFramework
    {
        public FrameworkData(TFramework framework)
        {
            Identifier = framework.Identifier;
            Version = framework.Version;
            Profile = framework.Profile;
            Framework = framework;
        }

        public string Identifier { get; }
        public Version Version { get; }
        public string Profile { get; }
        public TFramework Framework { get; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Profile))
            {
                return $"{Identifier},Version=v{GetDisplayVersion(Version)}";
            }
            else
            {
                return $"{Identifier},Version=v{GetDisplayVersion(Version)},Profile={Profile}";
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FrameworkData<TFramework>);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 27;
                hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Identifier);
                hash = (13 * hash) + Version.GetHashCode();
                hash = (13 * hash) + StringComparer.OrdinalIgnoreCase.GetHashCode(Profile);
                return hash;
            }
        }

        public bool Equals(FrameworkData<TFramework> other)
        {
            if (other == null)
            {
                return false;
            }

            return StringComparer.OrdinalIgnoreCase.Equals(Identifier, other.Identifier) &&
                   Version == other.Version &&
                   StringComparer.OrdinalIgnoreCase.Equals(Profile, other.Profile);
        }

        public int CompareTo(FrameworkData<TFramework> other)
        {
            var frameworkCompare = StringComparer.OrdinalIgnoreCase.Compare(Identifier, other.Identifier);
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
