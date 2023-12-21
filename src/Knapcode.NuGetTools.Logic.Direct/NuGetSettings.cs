using NuGet.Configuration;

namespace Knapcode.NuGetTools.Logic.Direct;

public class NuGetSettings
{
    public NuGetSettings(ISettings settings)
    {
        Settings = settings;
    }

    public ISettings Settings { get; }

    public string GlobalPackagesFolder
    {
        get
        {
            return SettingsUtility.GetGlobalPackagesFolder(Settings);
        }
        
        set
        {
            var path = Path.GetFullPath(value);
            SettingsUtility.SetConfigValue(Settings, "globalPackagesFolder", path);
        }
    }
}
