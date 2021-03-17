using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck
{
    /// <summary>
    /// Global configuration for LoggerHealthCheck.
    /// </summary>
    public class HealthCheckLoggerProviderConfiguration
    {
        /// <summary>
        /// Maximum number of LogEntries that will be stored, the default value is 100.
        /// </summary>
        public int MaxNumberOfLogEntries { get; set; } = 100;
        /// <summary>
        /// How long to keep LogEntries the default is 5 minutes.
        /// </summary>
        public TimeSpan FlushTime { get; set; } = TimeSpan.FromMinutes(5);
        /// <summary>
        /// What <see cref="LogLevel"/> is required to affect HealthChecks.
        /// </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Warning;
        /// <summary>
        /// Global filtration used to ignore events that should not be used to determine the status of health checks, default is <see cref="Filters.DefaultGlobalFilter"/>.
        /// </summary>
        public Func<LogEntry, bool> Filter { get; set; } = Filters.DefaultGlobalFilter;
    }
}
