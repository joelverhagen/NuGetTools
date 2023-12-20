using Knapcode.NuGetTools.Logic.Models;
using Knapcode.NuGetTools.Logic.Models.Framework;
using Knapcode.NuGetTools.Logic.Wrappers;

namespace Knapcode.NuGetTools.Logic
{
    public class FrameworkPrecedenceService : IFrameworkPrecedenceService
    {
        private readonly IFrameworkList _frameworkList;
        private readonly IFrameworkLogic _logic;

        public FrameworkPrecedenceService(string version, IFrameworkList frameworkList, IFrameworkLogic logic)
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
                Input = input,
                Precedence = Array.Empty<IFramework>(),
            };

            if (input != null &&
                !string.IsNullOrWhiteSpace(input.Framework))
            {
                try
                {
                    output.Framework = _logic.Parse(input.Framework);
                    output.InputStatus = InputStatus.Valid;
                }
                catch (Exception)
                {
                    output.InputStatus = InputStatus.Invalid;
                }
            }

            if (output.Framework is not null)
            {
                output.Precedence = GetPrecendence(output, output.Framework);
            }

            return output;
        }

        private IReadOnlyList<IFramework> GetPrecendence(FrameworkPrecedenceOutput output, IFramework framework)
        {
            // Get the initial set of candidates.
            var remainingCandidates = new HashSet<IFramework>(
                GetCandidates(output, framework),
                new FrameworkEqualityComparer());

            // Perform "get nearest" on the remaining set to find the next in precedence.
            var precedence = new List<IFramework>();
            while (remainingCandidates.Count > 0)
            {
                var nearest = _logic.GetNearest(framework, remainingCandidates);
                if (nearest is null)
                {
                    continue;
                }

                precedence.Add(nearest);
                remainingCandidates.Remove(nearest);
            }

            return precedence;
        }

        private IEnumerable<IFramework> GetCandidates(FrameworkPrecedenceOutput output, IFramework framework)
        {
            IEnumerable<IFramework> candidates = GetFrameworkList();

            if (!output.Input.IncludeProfiles)
            {
                candidates = candidates.Where(x => string.IsNullOrEmpty(x.Profile) || IsPortable(x));
            }

            if (output.Input.ExcludePortable)
            {
                candidates = candidates.Where(x => !IsPortable(x));
            }

            var excludedIdentifiers = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(output.Input.ExcludedIdentifiers))
            {
                var split = output
                    .Input
                    .ExcludedIdentifiers
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

        private static bool IsPortable(IFramework x)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(".NETPortable", x.Identifier);
        }

        private IReadOnlyList<IFramework> GetFrameworkList()
        {
            var frameworks = new List<IFramework>();

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

        private class FrameworkEqualityComparer : IEqualityComparer<IFramework>
        {
            public bool Equals(IFramework? x, IFramework? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return StringComparer.OrdinalIgnoreCase.Equals(x.DotNetFrameworkName, y.DotNetFrameworkName);
            }

            public int GetHashCode(IFramework obj)
            {
                return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.DotNetFrameworkName);
            }
        }
    }
}
