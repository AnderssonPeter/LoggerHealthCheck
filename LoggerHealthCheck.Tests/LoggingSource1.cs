using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck.Tests
{
    public class LoggingSource1
    {
        private readonly ILogger<LoggingSource1> logger;

        public LoggingSource1(ILogger<LoggingSource1> logger)
        {
            this.logger = logger;
        }

        public void ThrowException()
        {
            throw new Exception("Kaboom");
        }

        public void LogException(LogLevel logLevel = LogLevel.Error, string message = "")
        {
            try
            {
                throw new Exception("Kaboom");
            }
            catch(Exception ex)
            {
                logger.Log(logLevel, ex, message);
            }
        }

        public void Log(LogLevel logLevel, string message)
        {
            logger.Log(logLevel, message);
        }
    }
}
