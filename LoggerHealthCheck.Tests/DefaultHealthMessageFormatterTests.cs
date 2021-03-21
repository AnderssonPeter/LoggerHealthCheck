using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Xunit;

namespace LoggerHealthCheck.Tests
{
    public class DefaultHealthMessageFormatterTests
    {
        DefaultHealthMessageFormatter messageFormatter = new DefaultHealthMessageFormatter();
        
        [Fact]
        public void GroupBySourceIdenticalMessage()
        {
            var baseYear = new DateTime(1900, 01, 01, 01, 01, 01);
            var logEntries = Enumerable.Range(0, 2).Concat(new[] { 0, 0, 0 }).Select(i => new LogEntry(baseYear.AddSeconds(i), "Source" + i, LogLevel.Critical, "Template", "Message", new EventId(), null)).ToArray();
            var message = messageFormatter.GenerateMessage(logEntries, LogLevel.Error, TimeSpan.FromMinutes(5));
            Assert.Equal(@$"Critical: Source1 - Template
Message
Latest occurrence: {baseYear.AddSeconds(1)}, number of occurrences: 1 in the last 5 minutes

Critical: Source0 - Template
Message
Latest occurrence: {baseYear}, number of occurrences: 4 in the last 5 minutes", message.Content);
        }

        [Fact]
        public void GroupBySourceDiffrentMessage()
        {
            var messageCounter = 0;
            var baseYear = new DateTime(1900, 01, 01, 01, 01, 01);
            var logEntries = Enumerable.Range(0, 2).Concat(new[] { 0, 0, 0, 0 }).Select(i => new LogEntry(baseYear.AddSeconds(i), "Source" + i, LogLevel.Critical, "Template{0}", $"Message{messageCounter++}", new EventId(), null)).ToArray();
            var message = messageFormatter.GenerateMessage(logEntries, LogLevel.Error, TimeSpan.FromMinutes(5));
            Assert.Equal(@$"Critical: Source1 - Template{{0}}
Message1
Latest occurrence: {baseYear.AddSeconds(1)}, number of occurrences: 1 in the last 5 minutes

Critical: Source0 - Template{{0}}
Message0
Message2
Message3
And 2 more
Latest occurrence: {baseYear}, number of occurrences: 5 in the last 5 minutes", message.Content);
        }

        [InlineData(LogLevel.Critical, LogLevel.Critical, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Critical, LogLevel.Error, HealthStatus.Degraded)]
        [InlineData(LogLevel.Error, LogLevel.Critical, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Error, LogLevel.Error, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Error, LogLevel.Warning, HealthStatus.Degraded)]
        [InlineData(LogLevel.Warning, LogLevel.Critical, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Warning, LogLevel.Error, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Warning, LogLevel.Warning, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Warning, LogLevel.Information, HealthStatus.Degraded)]
        [Theory]
        public void CalculateHealthStatus(LogLevel unhealthyLogLevel, LogLevel logLevel, HealthStatus expectedHealthStatus)
        {
            var entries = new[] { new LogEntry(DateTime.Now, "Source", logLevel, "Template{0}", $"Message", new EventId(), null) };
            var result = messageFormatter.GenerateMessage(entries, unhealthyLogLevel, TimeSpan.Zero);
            Assert.Equal(expectedHealthStatus, result.Status);
        }
    }
}
