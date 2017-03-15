using System;
using System.Reflection;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection;
using Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api;

namespace Knapcode.NuGetTools.Logic
{
    public class AssemblyLoaderProxy : MarshalByRefObject
    {
        public FrameworkLogic GetFrameworkLogic(NuGetRelease release, AssemblyName assemblyName)
        {
            var frameworkApi = GetFrameworkApi(release, assemblyName);
            return new FrameworkLogic(frameworkApi);
        }

        public VersionLogic GetVersionLogic(NuGetRelease release, AssemblyName assemblyName)
        {
            var versionApi = GetVersionApi(release, assemblyName);
            return new VersionLogic(versionApi);
        }

        public VersionRangeLogic GetVersionRangeLogic(NuGetRelease release, AssemblyName versionAssemblyName, AssemblyName versionRangeAssemblyName)
        {
            var versionApi = GetVersionApi(release, versionAssemblyName);
            var versionRangeApi = GetVersionRangeApi(release, versionRangeAssemblyName);
            return new VersionRangeLogic(versionApi, versionRangeApi);
        }

        private IFrameworkApi GetFrameworkApi(NuGetRelease release, AssemblyName assemblyName)
        {
            switch (release)
            {
                case NuGetRelease.Version2x:
                    return new FrameworkApi2x(assemblyName);

                case NuGetRelease.Version3x:
                    return new FrameworkApi3x(assemblyName);

                default:
                    throw new NotImplementedException();
            }
        }

        private IVersionApi GetVersionApi(NuGetRelease release, AssemblyName assemblyName)
        {
            switch (release)
            {
                case NuGetRelease.Version2x:
                    return new VersionApi2x(assemblyName);

                case NuGetRelease.Version3x:
                    return new VersionApi3x(assemblyName);

                default:
                    throw new NotImplementedException();
            }
        }

        private IVersionRangeApi GetVersionRangeApi(NuGetRelease release, AssemblyName assemblyName)
        {
            switch (release)
            {
                case NuGetRelease.Version2x:
                    return new VersionRangeApi2x(assemblyName);

                case NuGetRelease.Version3x:
                    return new VersionRangeApi3x(assemblyName);

                default:
                    throw new NotImplementedException();
            }
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
