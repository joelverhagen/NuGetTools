using System;
using System.Collections.Generic;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic
{
    public class AppDomainContext
    {
        public AppDomainContext(string id, AppDomain appDomain, AssemblyLoaderProxy proxy)
        {
            Id = id;
            AppDomain = appDomain;
            Proxy = proxy;
            LoadedAssemblies = new List<AssemblyName>();
        }

        public string Id { get; }
        public AppDomain AppDomain { get; }
        public AssemblyLoaderProxy Proxy { get; }
        public IList<AssemblyName> LoadedAssemblies { get; }
    }
}
