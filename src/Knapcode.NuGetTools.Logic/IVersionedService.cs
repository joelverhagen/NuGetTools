using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Knapcode.NuGetTools.Logic
{
    public interface IVersionedService
    {
        string Version { get; }
    }
}
