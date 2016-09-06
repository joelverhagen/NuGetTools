using System;
using System.Collections.Generic;
using System.Linq;
using Knapcode.NuGetTools.Logic.Models;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class FrameworkPrecedenceService<TFramework> : IFrameworkPrecedenceService
        where TFramework : IFramework
    {
        private readonly IFrameworkList _frameworkList;
        private readonly IFrameworkLogic<TFramework> _logic;
        private readonly Lazy<IReadOnlyList<TFramework>> _parsedFrameworkList;

        public FrameworkPrecedenceService(string version, IFrameworkList frameworkList, IFrameworkLogic<TFramework> logic)
        {
            Version = version;
            _frameworkList = frameworkList;
            _logic = logic;
            _parsedFrameworkList = new Lazy<IReadOnlyList<TFramework>>(GetFrameworkList);
        }

        public string Version { get; }

        public FrameworkPrecedenceOutput FrameworkPrecedence(FrameworkPrecedenceInput input)
        {
            var output = new FrameworkPrecedenceOutput
            {
                InputStatus = InputStatus.Missing,
                Input = input
            };

            TFramework framework = default(TFramework);
            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Framework))
            {
                try
                {
                    framework = _logic.Parse(input.Framework);
                    output.Framework = framework;
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            if (output.InputStatus == InputStatus.Valid)
            {
                output.Precedence = GetPrecendence(framework);
            }

            return output;
        }

        private IReadOnlyList<IFramework> GetPrecendence(TFramework framework)
        {
            // Narrow the list of frameworks down to those that are compatible.
            var compatible = _parsedFrameworkList
                .Value
                .Where(x => _logic.IsCompatible(framework, x));
            var remainingCompatible = new HashSet<TFramework>(compatible, new FrameworkEqualityComparer());
            var precedence = new List<IFramework>();

            // Perform "get nearest" on the remaining set.
            while (remainingCompatible.Count > 0)
            {
                var nearest = _logic.GetNearest(framework, remainingCompatible);
                precedence.Add(nearest);
                remainingCompatible.Remove(nearest);
            }

            return precedence;
        }

        private IReadOnlyList<TFramework> GetFrameworkList()
        {
            var frameworks = new List<TFramework>();

            foreach (var item in _frameworkList.DotNetFrameworkNames)
            {
                try
                {
                    frameworks.Add(_logic.Parse(item));
                }
                catch
                {
                    // Ignore frameworks that cannot be parsed
                }
            }

            return frameworks;
        }

        [Serializable]
        private class FrameworkEqualityComparer : IEqualityComparer<TFramework>
        {
            public bool Equals(TFramework x, TFramework y)
            {
                return StringComparer.OrdinalIgnoreCase.Equals(x.DotNetFrameworkName, y.DotNetFrameworkName);
            }

            public int GetHashCode(TFramework obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.DotNetFrameworkName);
            }
        }
    }
}
