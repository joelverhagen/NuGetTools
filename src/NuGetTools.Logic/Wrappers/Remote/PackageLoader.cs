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

        public IReadOnlyList<AssemblyName> LoadPackageAssemblies(
            string appDomainName,
            NuGetFramework framework,
            PackageIdentity packageIdentity)
        {
            var pathResolver = new VersionFolderPathResolver(_settings.GlobalPackagesFolder);
            var hashPath = pathResolver.GetHashPath(packageIdentity.Id, packageIdentity.Version);

            if (!File.Exists(hashPath))
            {
                throw new ArgumentException($"The package {packageIdentity} could not found.", nameof(packageIdentity));
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
                if (!_appDomains.TryGetValue(appDomainName, out appDomainContext))
                {
                    var proxyType = typeof(Proxy);
                    var appDomainSetup = new AppDomainSetup
                    {
                        ApplicationBase = Path.GetDirectoryName(proxyType.Assembly.Location)
                    };
                    var evidence = AppDomain.CurrentDomain.Evidence;
                    var appDomain = AppDomain.CreateDomain(
                        appDomainName + ' ' + Guid.NewGuid(),
                        evidence,
                        appDomainSetup);

                    var proxy = (Proxy)appDomain.CreateInstanceAndUnwrap(
                        proxyType.Assembly.FullName,
                        proxyType.FullName);

                    appDomainContext = new AppDomainContext
                    {
                        AppDomain = appDomain,
                        Proxy = proxy
                    };

                    _appDomains[appDomainName] = appDomainContext;
                }

                var assemblyNames = new List<AssemblyName>();
                foreach (var asset in runtimeGroup.Items)
                {
                    var absolutePath = Path.Combine(installPath, asset.Path.Replace(AssetDirectorySeparator, Path.DirectorySeparatorChar));
                    var assemblyName = appDomainContext.Proxy.TryLoadAssembly(absolutePath);
                    assemblyNames.Add(assemblyName);
                }

                return assemblyNames;
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
