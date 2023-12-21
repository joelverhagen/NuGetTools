using NuGet.Common;

namespace Knapcode.NuGetTools.Logic.Direct;

public class ConsoleLogger : ILogger
{
    public void Log(LogLevel level, string data)
    {
        if (level < LogLevel.Information)
        {
            return;
        }

        Console.WriteLine(data);
    }

    public void Log(ILogMessage message)
    {
        Log(message.Level, message.Message);
    }

    public Task LogAsync(LogLevel level, string data)
    {
        Log(level, data);
        return Task.CompletedTask;
    }

    public Task LogAsync(ILogMessage message)
    {
        Log(message);
        return Task.CompletedTask;
    }

    public void LogDebug(string data)
    {
        Log(LogLevel.Debug, data);
    }

    public void LogError(string data)
    {
        Log(LogLevel.Error, data);
    }

    public void LogErrorSummary(string data)
    {
        Log(LogLevel.Error, data);
    }

    public void LogInformation(string data)
    {
        Log(LogLevel.Information, data);
    }

    public void LogInformationSummary(string data)
    {
        Log(LogLevel.Information, data);
    }

    public void LogMinimal(string data)
    {
        Log(LogLevel.Minimal, data);
    }

    public void LogVerbose(string data)
    {
        Log(LogLevel.Verbose, data);
    }

    public void LogWarning(string data)
    {
        Log(LogLevel.Warning, data);
    }
}
