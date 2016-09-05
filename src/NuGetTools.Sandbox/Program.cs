using System;

namespace Knapcode.NuGetTools.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var enumerator = new FrameworkEnumerator();
            foreach (var framework in enumerator.Enumerate(FrameworkEnumeratorOptions.All))
            {
                Console.WriteLine(framework);
            }
        }
    }
}
