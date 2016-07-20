using System.IO;

namespace Knapcode.NuGetTools.Build
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var outputDirectory = Directory.GetCurrentDirectory();

            // Discover the assembly info from Git.
            var assemblyInfo = AssemblyInfoWriter.DiscoverAssemblyInfo(outputDirectory);

            // Write the assembly info.
            var propertiesDirectory = Path.Combine(outputDirectory, "Properties");
            Directory.CreateDirectory(propertiesDirectory);

            var assemblyInfoPath = Path.Combine(propertiesDirectory, "AssemblyInfo.cs");
            AssemblyInfoWriter.WriteAssemblyInfo(assemblyInfoPath, assemblyInfo);            
        }
    }
}
