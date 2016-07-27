using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class AppDomainContext
    {
        public AppDomain AppDomain { get; set; }
        public Proxy Proxy { get; set; }
    }
}
