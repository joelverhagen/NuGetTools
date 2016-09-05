using System;
using System.Collections.Generic;
using System.Linq;

namespace Knapcode.NuGetTools.Build
{
    public class CommandResult
    {
        public Command Command { get; set; }
        public CommandStatus Status { get; set; }
        public Exception Exception { get; set; }
        public IEnumerable<CommandOutputLine> Lines { get; set; }
        public int ExitCode { get; set; }

        public void EnsureSuccess()
        {
            if (Status == CommandStatus.Exited)
            {
                if (ExitCode == 0)
                {
                    return;
                }

                throw new CommandException($"The command '{Command?.GetDisplayString()}' failed with exit code {ExitCode}.");
            }
            else if (Status == CommandStatus.FailedToStartCommand)
            {
                throw new CommandException($"The command '{Command?.GetDisplayString()}' failed to started.", Exception);
            }
            else if (Status == CommandStatus.Timeout || Status == CommandStatus.FailedToKillAfterTimeout)
            {
                throw new CommandException($"The command '{Command?.GetDisplayString()}' timed out after {Command?.Timeout.TotalSeconds} seconds.", Exception);
            }
        }
        
        public IEnumerable<string> OutputLines
        {
            get
            {
                if (Lines == null)
                {
                    return Enumerable.Empty<string>();
                }

                return Lines
                    .Where(x => x.Type == CommandOutputLineType.StandardOutput)
                    .Select(x => x.Value);
            }
        }

        public IEnumerable<string> ErrorLines
        {
            get
            {
                if (Lines == null)
                {
                    return Enumerable.Empty<string>();
                }

                return Lines
                    .Where(x => x.Type == CommandOutputLineType.StandardError)
                    .Select(x => x.Value);
            }
        }

        public string Output
        {
            get
            {
                return string.Join(Environment.NewLine, OutputLines);
            }
        }

        public string Error
        {
            get
            {
                return string.Join(Environment.NewLine, ErrorLines);
            }
        }
    }
}
