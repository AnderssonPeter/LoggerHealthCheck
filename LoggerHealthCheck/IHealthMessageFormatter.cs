using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;

namespace LoggerHealthCheck
{
    public interface IHealthMessageFormatter
    {
        public HealthMessage GenerateMessage(LogEntry[] entries, LogLevel unhealthyLogLevel, TimeSpan flushTime);
    }
}
