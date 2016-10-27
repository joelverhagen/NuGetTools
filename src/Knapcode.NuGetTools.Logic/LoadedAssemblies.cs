using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic
{
    public class LoadedAssemblies
    {
        private readonly object _lock = new object();
        private readonly List<AssemblyName> _assemblyNames = new List<AssemblyName>();

        public AssemblyName GetByName(string name)
        {
            lock (_lock)
            {
                var assemblyName = _assemblyNames.FirstOrDefault(x => x.Name == name);
                if (assemblyName == null)
                {
                    throw new KeyNotFoundException($"The assembly with name '{name}' could not be found.");
                }

                return assemblyName;
            }
        }

        public void Add(AssemblyName assemblyName)
        {
            lock (_lock)
            {
                _assemblyNames.Add(assemblyName);
            }
        }
    }
}
