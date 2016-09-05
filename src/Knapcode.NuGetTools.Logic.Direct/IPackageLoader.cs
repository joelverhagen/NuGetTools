using System;
using NuGet.Frameworks;
using NuGet.Packaging.Core;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public interface IPackageLoader : IDisposable
    {
        AppDomainContext LoadPackageAssemblies(string appDomainId, NuGetFramework framework, PackageIdentity packageIdentity);
    }
}