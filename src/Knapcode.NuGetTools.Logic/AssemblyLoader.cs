using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic
{
    public class AssemblyLoader : IAssemblyLoader
    {
        private readonly object _appDomainsLock = new object();
        private readonly Dictionary<string, AppDomainContext> _appDomains
            = new Dictionary<string, AppDomainContext>();

        public AppDomainContext GetAppDomainContext(string appDomainId)
        {
            lock (_appDomainsLock)
            {
                // Initialize the app domain if necessary.
                AppDomainContext appDomainContext;
                if (!_appDomains.TryGetValue(appDomainId, out appDomainContext))
                {
                    var proxyType = typeof(AssemblyLoaderProxy);
                    var appDomainSetup = new AppDomainSetup
                    {
                        ApplicationBase = Path.GetDirectoryName(proxyType.Assembly.Location)
                    };
                    var evidence = AppDomain.CurrentDomain.Evidence;
                    var appDomain = AppDomain.CreateDomain(
                        appDomainId + ' ' + Guid.NewGuid(),
                        evidence,
                        appDomainSetup);

                    var proxy = (AssemblyLoaderProxy)appDomain.CreateInstanceAndUnwrap(
                        proxyType.Assembly.FullName,
                        proxyType.FullName);

                    appDomainContext = new AppDomainContext(appDomainId, appDomain, proxy);
                    _appDomains[appDomainId] = appDomainContext;
                }

                return appDomainContext;
            }
        }

        public AssemblyName LoadAssembly(AppDomainContext context, string assemblyPath)
        {
            if (!File.Exists(assemblyPath))
            {
                throw new InvalidOperationException($"The assembly '{assemblyPath}' could not found.");
            }

            // Load the assembly
            var assemblyName = context.Proxy.LoadAssembly(assemblyPath);

            context.LoadedAssemblies.Add(assemblyName);

            return assemblyName;
        }

        public void Dispose()
        {
            foreach (var context in _appDomains)
            {
                try
                {
                    AppDomain.Unload(context.Value.AppDomain);
                }
                catch
                {
                    // Nothing we can do here.
                }
            }
        }
    }
}
