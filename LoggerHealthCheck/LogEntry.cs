using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck
{
    public record LogEntry(DateTime Timestamp, string Source, LogLevel LogLevel, string? MessageTemplate, string Message, EventId EventId, Exception? Exception);
}
