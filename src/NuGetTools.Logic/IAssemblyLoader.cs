using System;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public interface IAssemblyLoader : IDisposable
    {
        AppDomainContext GetAppDomainContext(string appDomainId);
        AssemblyName LoadAssembly(AppDomainContext context, string assemblyPath);
    }
}