using Microsoft.Extensions.Logging;

namespace Knapcode.NuGetTools.Logic.Direct
{
    public class MicrosoftLogger : NuGet.Common.ILogger
    {
        private readonly ILogger _logger;

        public MicrosoftLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void Log(NuGet.Common.LogLevel level, string data)
        {
            switch (level)
            {
                case NuGet.Common.LogLevel.Debug:
                    _logger.LogTrace(data);
                    break;
                case NuGet.Common.LogLevel.Verbose:
                    _logger.LogDebug(data);
                    break;
                case NuGet.Common.LogLevel.Information:
                    _logger.LogInformation(data);
                    break;
                case NuGet.Common.LogLevel.Minimal:
                    _logger.LogInformation(data);
                    break;
                case NuGet.Common.LogLevel.Warning:
                    _logger.LogWarning(data);
                    break;
                case NuGet.Common.LogLevel.Error:
                    _logger.LogError(data);
                    break;
                default:
                    _logger.LogInformation(data);
                    break;
            }
        }

        public void Log(NuGet.Common.ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        public Task LogAsync(NuGet.Common.LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;
        }

        public Task LogAsync(NuGet.Common.ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        public void LogDebug(string data)
        {
            _logger.LogTrace(data);
        }

        public void LogError(string data)
        {
            _logger.LogError(data);
        }

        public void LogErrorSummary(string data)
        {
            _logger.LogError(data);
        }

        public void LogInformation(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogInformationSummary(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogMinimal(string data)
        {
            _logger.LogInformation(data);
        }

        public void LogVerbose(string data)
        {
            _logger.LogDebug(data);
        }

        public void LogWarning(string data)
        {
            _logger.LogWarning(data);
        }
    }
}
