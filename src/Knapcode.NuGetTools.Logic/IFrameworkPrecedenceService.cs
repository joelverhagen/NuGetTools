using Knapcode.NuGetTools.Logic.Models.Framework;

namespace Knapcode.NuGetTools.Logic;

public interface IFrameworkPrecedenceService : IVersionedService
{
    FrameworkPrecedenceOutput FrameworkPrecedence(FrameworkPrecedenceInput input);
}