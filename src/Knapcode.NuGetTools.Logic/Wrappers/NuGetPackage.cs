namespace Knapcode.NuGetTools.Logic.Wrappers;

public record NuGetPackage(
    string Id,
    string Version,
    IReadOnlyList<NuGetAssembly> Assemblies);
