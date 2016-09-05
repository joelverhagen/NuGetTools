using System;
using System.Globalization;
using System.Text;
using NuGet.Frameworks;

namespace Knapcode.NuGetTools.Sandbox
{
    public class FrameworkData : IEquatable<FrameworkData>, IComparable<FrameworkData>
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
