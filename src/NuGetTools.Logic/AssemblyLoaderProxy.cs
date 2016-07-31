using System;
using System.Reflection;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic
{
    public class AssemblyLoaderProxy : MarshalByRefObject
    {
        public FrameworkLogic GetFrameworkLogic(AssemblyName assemblyName)
        {
            var frameworkApi = new FrameworkApi(assemblyName);
            return new FrameworkLogic(frameworkApi);
        }

        public VersionLogic GetVersionLogic(AssemblyName assemblyName)
        {
            var versionApi = new VersionApi(assemblyName);
            return new VersionLogic(versionApi);
        }

        public VersionRangeLogic GetVersionRangeLogic(AssemblyName assemblyName)
        {
            var versionApi = new VersionApi(assemblyName);
            var versionRangeApi = new VersionRangeApi(assemblyName);
            return new VersionRangeLogic(versionApi, versionRangeApi);
        }

        public AssemblyName LoadAssembly(string assemblyPath)
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
