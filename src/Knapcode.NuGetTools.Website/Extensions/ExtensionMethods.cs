using Microsoft.AspNetCore.Hosting;

namespace Knapcode.NuGetTools.Website
{
    public static class ExtensionMethods
    {
        public static bool IsAutomation(this IHostingEnvironment env)
        {
            return env.IsEnvironment("Automation");
        }
    }
}
