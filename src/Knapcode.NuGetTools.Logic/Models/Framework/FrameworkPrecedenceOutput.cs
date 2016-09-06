using System.Collections.Generic;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic.Models.Framework
{
    public class FrameworkPrecedenceOutput
    {
        public InputStatus InputStatus { get; set; }
        public FrameworkPrecedenceInput Input { get; set; }
        public IFramework Framework { get; set; }
        public IReadOnlyList<IFramework> Precedence { get; set; }
    }
}