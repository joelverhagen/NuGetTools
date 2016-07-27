namespace Knapcode.NuGetTools.Logic.Wrappers
{
    public interface IVersionRange
    {
        string NormalizedString { get; }
        bool IsFloating { get; }
        string PrettyPrint { get; }
        bool HasLowerBound { get; }
        bool HasUpperBound { get; }
        bool IsMinInclusive { get; }
        bool IsMaxInclusive { get; }
        IVersion MinVersion { get; }
        IVersion MaxVersion { get; }
    }
}
