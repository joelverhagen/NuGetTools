namespace Knapcode.NuGetTools.Logic;

public class FrameworkListItem
{
    public FrameworkListItem(string identifier, Version version, string profile, string dotNetFrameworkName, string shortFolderName)
    {
        Identifier = identifier;
        Version = version;
        Profile = profile;
        DotNetFrameworkName = dotNetFrameworkName;
        ShortFolderName = shortFolderName;
    }

    public string Identifier { get; }
    public Version Version { get; }
    public string Profile { get; }
    public string DotNetFrameworkName { get; }
    public string ShortFolderName { get; }
}
