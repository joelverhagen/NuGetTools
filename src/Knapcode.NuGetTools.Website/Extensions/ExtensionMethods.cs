namespace Microsoft.Extensions.Hosting;

public static class ExtensionMethods
{
    public static bool IsAutomation(this IHostEnvironment env)
    {
        return env.IsEnvironment("Automation");
    }
}
