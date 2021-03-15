using LoggerHealthCheck;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck
{
    /// <summary>
    /// Provides configuration for <see cref="LoggerHealthCheck"/>
    /// </summary>
    public class LoggerHealthCheckOptions
    {
        /// <summary>
        /// What <see cref="LogLevel"/> is required to mark the healtcheck as <see cref="HealthStatus.Unhealthy"/>, default is <see cref="LogLevel.Error"/>.
        /// </summary>
        public LogLevel UnhealthyLogLevel { get; set; } = LogLevel.Error;

        /// <summary>
        /// Filter used to determine what log events will affect the HealthCheck
        /// </summary>
        public Func<LogEntry, bool> Filter { get; set; } = Filters.DefaultHealthCheck;
    }
}
