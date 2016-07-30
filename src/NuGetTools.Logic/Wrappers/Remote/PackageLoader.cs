using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NuGet.Client;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class PackageLoader : IDisposable
    {
        private const char AssetDirectorySeparator = '/';
        private readonly NuGetSettings _settings;
        private readonly Dictionary<string, AppDomainContext> _appDomains
            = new Dictionary<string, AppDomainContext>();

        public PackageLoader(NuGetSettings settings)
        {
            _settings = settings;
        }

        public AppDomainContext LoadPackageAssemblies(
            string appDomainId,
            NuGetFramework framework,
            PackageIdentity packageIdentity)
        {
            var pathResolver = new VersionFolderPathResolver(_settings.GlobalPackagesFolder);
            var hashPath = pathResolver.GetHashPath(packageIdentity.Id, packageIdentity.Version);

            if (!File.Exists(hashPath))
            {
                throw new InvalidOperationException($"The package {packageIdentity} could not found.");
            }

            var installPath = pathResolver.GetInstallPath(packageIdentity.Id, packageIdentity.Version);

            using (var packageReader = new PackageFolderReader(installPath))
            {
                var conventions = new ManagedCodeConventions(null);
                var criteria = conventions.Criteria.ForFramework(framework);

                var files = packageReader
                    .GetFiles()
                    .Select(p => p.Replace(Path.DirectorySeparatorChar, AssetDirectorySeparator))
                    .ToList();

                var contentItems = new ContentItemCollection();
                contentItems.Load(files);

                var runtimeGroup = contentItems.FindBestItemGroup(
                    criteria,
                    conventions.Patterns.RuntimeAssemblies);

                // Initialize the app domain if necessary.
                AppDomainContext appDomainContext;
                if (!_appDomains.TryGetValue(appDomainId, out appDomainContext))
                {
                    var proxyType = typeof(Proxy);
                    var appDomainSetup = new AppDomainSetup
                    {
                        ApplicationBase = Path.GetDirectoryName(proxyType.Assembly.Location)
                    };
                    var evidence = AppDomain.CurrentDomain.Evidence;
                    var appDomain = AppDomain.CreateDomain(
                        appDomainId + ' ' + Guid.NewGuid(),
                        evidence,
                        appDomainSetup);

                    var proxy = (Proxy)appDomain.CreateInstanceAndUnwrap(
                        proxyType.Assembly.FullName,
                        proxyType.FullName);

                    appDomainContext = new AppDomainContext(appDomainId, appDomain, proxy);
                    _appDomains[appDomainId] = appDomainContext;
                }
                
                foreach (var asset in runtimeGroup.Items)
                {
                    var absolutePath = Path.Combine(installPath, asset.Path.Replace(AssetDirectorySeparator, Path.DirectorySeparatorChar));
                    var assemblyName = appDomainContext.Proxy.TryLoadAssembly(absolutePath);
                    appDomainContext.LoadedAssemblies.Add(assemblyName);
                }

                return appDomainContext;
            }
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
