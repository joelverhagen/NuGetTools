namespace Knapcode.NuGetTools.Logic
{
    [Flags]
    public enum FrameworkExpansionOptions
    {
        None = 1 << 0,
        RoundTripDotNetFrameworkName = 1 << 1,
        RoundTripShortFolderName = 1 << 2,
        FrameworkExpander = 1 << 3,
        All = ~0
    }
}
