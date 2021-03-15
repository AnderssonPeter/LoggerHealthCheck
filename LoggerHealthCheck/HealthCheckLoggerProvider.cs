using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LoggerHealthCheck
{
    public class HealthCheckLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, HealthCheckLogger> loggers = new ConcurrentDictionary<string, HealthCheckLogger>();
        private readonly Stack<LogEntry> logEntries = new Stack<LogEntry>();
        public HealthCheckLoggerProvider(HealthCheckLoggerProviderConfiguration configuration)
        {
            Configuration = configuration;
        }

        public HealthCheckLoggerProviderConfiguration Configuration { get; }

        public ILogger CreateLogger(string categoryName) => loggers.GetOrAdd(categoryName, name => new HealthCheckLogger(name, this));

        public void AddLogEntry(LogEntry logEntry)
        {
            if (Configuration.Filter(logEntry))
            {
                lock(logEntries)
                {
                    while (logEntries.TryPeek(out var oldestEntry) && DateTime.Now.Subtract(oldestEntry.Timestamp) >= Configuration.FlushTime || 
                           logEntries.Count >= Configuration.MaxNumberOfLogEntries)
                    {
                        logEntries.Pop();
                    }
                    logEntries.Push(logEntry);
                }
            }
        }

        public LogEntry[] GetLogEntries()
        {
            lock (logEntries)
            {
                while (logEntries.TryPeek(out var oldestEntry) && DateTime.Now.Subtract(oldestEntry.Timestamp) >= Configuration.FlushTime)
                {
                    logEntries.Pop();
                }
                return logEntries.ToArray();
            }
        }

        public void Dispose() => loggers.Clear();
    }
}
