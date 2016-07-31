using System;
using System.IO;
using System.Linq;
using System.Threading;
using NuGet.Client;
using NuGet.ContentModel;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class PackageLoader : IDisposable
    {
        private const char AssetDirectorySeparator = '/';
        private AssemblyLoader _loader;
        private readonly NuGetSettings _settings;

        public PackageLoader(AssemblyLoader loader, NuGetSettings settings)
        {
            _loader = loader;
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
                throw new InvalidOperationException($"The package '{packageIdentity}' could not found.");
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

                var appDomainContext = _loader.GetAppDomainContext(appDomainId);

                foreach (var asset in runtimeGroup.Items)
                {
                    var absolutePath = Path.Combine(installPath, asset.Path.Replace(AssetDirectorySeparator, Path.DirectorySeparatorChar));
                    _loader.LoadAssembly(appDomainContext, absolutePath);
                }

                return appDomainContext;
            }
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _loader, null)?.Dispose();
        }
    }
}
