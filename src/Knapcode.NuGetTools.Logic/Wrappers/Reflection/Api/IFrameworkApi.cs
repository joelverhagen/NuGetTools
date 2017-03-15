using System;
using System.Collections.Generic;

namespace Knapcode.NuGetTools.Logic.Wrappers.Reflection.Api
{
    public interface IFrameworkApi
    {
        string GetDotNetFrameworkName(object nuGetFramework);
        string GetIdentifer(object nuGetFramework);
        object GetNearest(object project, IEnumerable<object> package);
        string GetProfile(object nuGetFramework);
        string GetShortFolderName(object nuGetFramework);
        System.Version GetVersion(object nuGetFramework);
        bool IsCompatible(object project, object package);
        object Parse(string input);
    }
}