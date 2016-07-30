using System;
using System.Reflection;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class Proxy : MarshalByRefObject
    {
        public ReflectionFrameworkLogic GetFrameworkLogic(AssemblyName assemblyName)
        {
            var frameworkApi = new FrameworkApi(assemblyName);
            return new ReflectionFrameworkLogic(frameworkApi);
        }

        public ReflectionVersionLogic GetVersionLogic(AssemblyName assemblyName)
        {
            var versionApi = new VersionApi(assemblyName);
            return new ReflectionVersionLogic(versionApi);
        }

        public ReflectionVersionRangeLogic GetVersionRangeLogic(AssemblyName assemblyName)
        {
            var versionApi = new VersionApi(assemblyName);
            var versionRangeApi = new VersionRangeApi(assemblyName);
            return new ReflectionVersionRangeLogic(versionApi, versionRangeApi);
        }

        public AssemblyName TryLoadAssembly(string assemblyPath)
        {
            try
            {
                var assembly = Assembly.LoadFile(assemblyPath);
                return assembly.GetName();
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    $"Assembly {assemblyPath} could not be loaded. Exception:" +
                    Environment.NewLine +
                    exception.ToString());
            }

        }
    }
}
