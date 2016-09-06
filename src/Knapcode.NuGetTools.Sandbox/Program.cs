using System;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;

namespace Knapcode.NuGetTools.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var enumerator = new FrameworkEnumerator();
            var enumerated = enumerator.Enumerate(FrameworkEnumerationOptions.All);
            var expanded = enumerator.Expand(enumerated, FrameworkExpansionOptions.All);

            foreach (var framework in expanded)
            {
                Console.WriteLine(framework);
            }
        }
    }
}
