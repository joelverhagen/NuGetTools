using System.Globalization;
using System.Text;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic;

public class FrameworkEnumeratorData : IEquatable<FrameworkEnumeratorData>, IComparable<FrameworkEnumeratorData>
{
    public FrameworkEnumeratorData(IFramework framework)
    {
        Identifier = framework.Identifier;
        Version = framework.Version;
        Profile = framework.Profile;
        Framework = framework;
    }

    public string Identifier { get; }
    public Version Version { get; }
    public string Profile { get; }
    public IFramework Framework { get; }

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

    public override bool Equals(object? obj)
    {
        return Equals(obj as FrameworkEnumeratorData);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Identifier, StringComparer.OrdinalIgnoreCase);
        hashCode.Add(Version);
        hashCode.Add(Profile, StringComparer.OrdinalIgnoreCase);
        return hashCode.ToHashCode();
    }

    public bool Equals(FrameworkEnumeratorData? other)
    {
        if (other == null)
        {
            return false;
        }

        return StringComparer.OrdinalIgnoreCase.Equals(Identifier, other.Identifier) &&
               Version == other.Version &&
               StringComparer.OrdinalIgnoreCase.Equals(Profile, other.Profile);
    }

    public int CompareTo(FrameworkEnumeratorData? other)
    {
        if (other is null)
        {
            return 1;
        }

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
