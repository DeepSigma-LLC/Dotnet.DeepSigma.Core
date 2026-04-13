
using Microsoft.Extensions.Logging;

namespace DeepSigma.Core
{
    internal class LoggerTest
    {
        private readonly ILogger<LoggerTest> _logger;
        internal LoggerTest(ILogger<LoggerTest> logger)
        {
            _logger = logger;
        }

        internal void LogMesssage()
        {
            _logger.LogInformation("LoggerTest instance created.");
            _logger.LogWarning("This is a warning message from LoggerTest.");
            _logger.LogError("This is an error message from LoggerTest.");
        }
    }
}
