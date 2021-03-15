using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck
{
    class HealthCheckLogger : ILogger
    {
        private readonly string categoryName;
        private readonly HealthCheckLoggerProvider healthCheckLoggerProvider;

        public HealthCheckLogger(string categoryName, HealthCheckLoggerProvider healthCheckLoggerProvider)
        {
            this.categoryName = categoryName;
            this.healthCheckLoggerProvider = healthCheckLoggerProvider;
        }
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => healthCheckLoggerProvider.Configuration.MinLogLevel <= logLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {                
                healthCheckLoggerProvider.AddLogEntry(new LogEntry(DateTime.Now, categoryName, logLevel, formatter(state, exception), eventId, exception));
            }
        }
    }
}
