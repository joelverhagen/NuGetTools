using System;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic
{
    public static class AssemblyNameExtensions
    {
        public static Assembly GetAssembly(this AssemblyName assemblyName)
        {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(x => x.GetName().FullName == assemblyName.FullName);
        }
    }
}
