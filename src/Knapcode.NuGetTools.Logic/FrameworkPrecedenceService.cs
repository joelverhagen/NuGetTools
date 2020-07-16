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

        public FrameworkPrecedenceService(string version, IFrameworkList frameworkList, IFrameworkLogic<TFramework> logic)
        {
            Version = version;
            _frameworkList = frameworkList;
            _logic = logic;
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
                output.Precedence = GetPrecendence(output, framework);
            }

            return output;
        }

        private IReadOnlyList<IFramework> GetPrecendence(FrameworkPrecedenceOutput output, TFramework framework)
        {
            // Get the initial set of candidates.
            var remainingCandidates = new HashSet<TFramework>(
                GetCandidates(output, framework),
                new FrameworkEqualityComparer());

            // Perform "get nearest" on the remaining set to find the next in precedence.
            var precedence = new List<IFramework>();
            while (remainingCandidates.Count > 0)
            {
                var nearest = _logic.GetNearest(framework, remainingCandidates);
                precedence.Add(nearest);
                remainingCandidates.Remove(nearest);
            }

            return precedence;
        }

        private IEnumerable<TFramework> GetCandidates(FrameworkPrecedenceOutput output, TFramework framework)
        {
            IEnumerable<TFramework> candidates = GetFrameworkList();

            if (!output.Input.IncludeProfiles)
            {
                candidates = candidates.Where(x => string.IsNullOrEmpty(x.Profile) || IsPortable(x));
            }

            if (output.Input.ExcludePortable)
            {
                candidates = candidates.Where(x => !IsPortable(x));
            }

            var excludedIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(output.Input.ExludedIdentifiers))
            {
                var split = output
                    .Input
                    .ExludedIdentifiers
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToList();

                foreach (var identifier in split)
                {
                    excludedIdentifiers.Add(identifier);
                }

                if (excludedIdentifiers.Any())
                {
                    candidates = candidates.Where(x => !excludedIdentifiers.Contains(x.Identifier));
                }
            }

            // Narrow the list of frameworks down to those that are compatible.
            candidates = candidates.Where(x => _logic.IsCompatible(framework, x));

            return candidates;
        }

        private static bool IsPortable(TFramework x)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(".NETPortable", x.Identifier);
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
