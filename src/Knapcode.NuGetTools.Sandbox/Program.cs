using System;
using Knapcode.NuGetTools.Logic;
using Knapcode.NuGetTools.Logic.Direct;
using Knapcode.NuGetTools.Logic.Direct.Wrappers;

namespace Knapcode.NuGetTools.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var enumerator = new FrameworkEnumerator();
            var frameworkList = new FrameworkList<Framework>(enumerator);
            
            foreach (var dotNetFrameworkName in frameworkList.DotNetFrameworkNames)
            {
                Console.WriteLine(dotNetFrameworkName);
            }
        }
    }
}
