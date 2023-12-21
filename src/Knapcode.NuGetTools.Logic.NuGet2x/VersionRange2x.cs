using Knapcode.NuGetTools.Logic.Wrappers;
using NuGet;

namespace Knapcode.NuGetTools.Logic.NuGet2x;

public class VersionRange2x : IVersionRange
{
    public VersionRange2x(IVersionSpec versionSpec)
    {
        VersionSpec = versionSpec;
    }

    public IVersionSpec VersionSpec { get; }

    public string NormalizedString => VersionSpec.ToString()!;
    public bool IsFloating => throw new NotSupportedException();
    public string PrettyPrint => VersionUtility.PrettyPrint(VersionSpec);
    public bool HasLowerBound => VersionSpec.MinVersion is not null;
    public bool HasUpperBound => VersionSpec.MaxVersion is not null;
    public bool IsMinInclusive => VersionSpec.IsMinInclusive;
    public bool IsMaxInclusive => VersionSpec.IsMaxInclusive;
    public IVersion MinVersion => new Version2x(VersionSpec.MinVersion);
    public IVersion MaxVersion => new Version2x(VersionSpec.MaxVersion);
    public string LegacyShortString => throw new NotSupportedException();
    public string LegacyString => throw new NotSupportedException();
    public string OriginalString => throw new NotSupportedException();
    public bool LegacyShortStringAvailable => false;
    public bool IsFloatingAvailable => false;
    public bool OriginalStringAvailable => false;
    public bool LegacyStringAvailable => false;
}
