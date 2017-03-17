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
