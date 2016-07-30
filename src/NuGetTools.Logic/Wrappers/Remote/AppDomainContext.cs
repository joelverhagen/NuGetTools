using System;
using System.Collections.Generic;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class AppDomainContext
    {
        public AppDomainContext(string id, AppDomain appDomain, Proxy proxy)
        {
            Id = id;
            AppDomain = appDomain;
            Proxy = proxy;
            LoadedAssemblies = new List<AssemblyName>();
        }

        public string Id { get; }
        public AppDomain AppDomain { get; }
        public Proxy Proxy { get; }
        public IList<AssemblyName> LoadedAssemblies { get; }
    }
}
