using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace LoggerHealthCheck
{
    class HealthCheckLogger : ILogger
    {
        private const string TemplateName = "{OriginalFormat}";
        private readonly string categoryName;
        private readonly HealthCheckLoggerProvider healthCheckLoggerProvider;

        public HealthCheckLogger(string categoryName, HealthCheckLoggerProvider healthCheckLoggerProvider)
        {
            this.categoryName = categoryName;
            this.healthCheckLoggerProvider = healthCheckLoggerProvider;
        }
        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => healthCheckLoggerProvider.Configuration.MinLogLevel <= logLevel;

        private string? GetTemplate<TState>(TState state)
        {
            if (state is IEnumerable<KeyValuePair<string, object>> structure)
            {
                foreach (var property in structure)
                {
                    if (property.Key == TemplateName && property.Value is string value)
                    {
                       return value;
                    }
                }
            }
            return null;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception?, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                var template = GetTemplate(state);
                healthCheckLoggerProvider.AddLogEntry(new LogEntry(DateTime.Now, categoryName, logLevel, template, formatter(state, exception), eventId, exception));
            }
        }
    }
}
