using System;

namespace Knapcode.NuGetTools.Sandbox
{
    [Flags]
    public enum FrameworkEnumeratorOptions
    {
        None = 1 << 0,
        FrameworkNameProvider = 1 << 1,
        CommonFrameworks = 1 << 2,
        FrameworkMappings = 1 << 3,
        PortableFrameworkMappings = 1 << 4,
        SpecialFrameworks = 1 << 5,
        RoundTripDotNetFrameworkName = 1 << 6,
        RoundTripShortFolderName = 1 << 7,
        FrameworkExpander = 1 << 8,
        All = ~0
    }
}
