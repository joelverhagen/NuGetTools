using System;
using System.Reflection;

namespace Knapcode.NuGetTools.Logic.Wrappers.Remote
{
    public class Proxy : MarshalByRefObject
    {
        public string GetString(string typeName, object[] args, string methodName, Type[] methodArgTypes, object[] methodArgs)
        {
            var type = Type.GetType(typeName);

            object instance = null;
            if (args != null)
            {
                instance = Activator.CreateInstance(type, args);
            }

            var method = type.GetMethod(methodName, methodArgTypes);
            var result = method.Invoke(instance, methodArgs);

            return result.ToString();
        }

        public AssemblyName TryLoadAssembly(string assemblyPath)
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
