using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;

namespace Knapcode.NuGetTools.Build
{
    public class CommandRunner
    {
        public CommandResult Run(Command command)
        {
            var process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = command.WorkingDirectory,
                    FileName = command.FileName,
                    Arguments = command.Arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            foreach (var pair in command.Environment)
            {
                process.StartInfo.Environment[pair.Key] = pair.Value;
            }

            using (process)
            {
                var queue = new ConcurrentQueue<CommandOutputLine>();

                process.OutputDataReceived += (sender, e) =>
                {
                    queue.Enqueue(new CommandOutputLine(
                        CommandOutputLineType.StandardOutput,
                        e.Data));
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    queue.Enqueue(new CommandOutputLine(
                        CommandOutputLineType.StandardError,
                        e.Data));
                };

                var status = CommandStatus.Exited;
                Exception exception = null;
                var exitCode = -1;
                bool started;

                try
                {
                    process.Start();
                    started = true;
                }
                catch (Exception e)
                {
                    status = CommandStatus.FailedToStartCommand;
                    exception = e;
                    started = false;
                }

                if (started)
                {
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    var exited = process.WaitForExit((int)command.Timeout.TotalMilliseconds);

                    if (!exited)
                    {
                        try
                        {
                            process.Kill();
                            status = CommandStatus.Timeout;
                        }
                        catch (Exception e)
                        {
                            exception = e;
                            status = CommandStatus.FailedToKillAfterTimeout;
                            // Nothing else we can do here.
                        }
                    }
                    else
                    {
                        exitCode = process.ExitCode;
                    }
                }

                return new CommandResult
                {
                    Command = command,
                    Status = status,
                    Exception = exception,
                    Lines = queue.ToList(),
                    ExitCode = exitCode
                };
            }
        }
    }
}
