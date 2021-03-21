using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LoggerHealthCheck.Tests
{
    public class HealthCheckLoggerProviderTests
    {
        [Fact]
        public void LimitWithMaxNumber()
        {
            var healthCheckLoggerProvider = new HealthCheckLoggerProvider(new HealthCheckLoggerProviderConfiguration
            {
                MaxNumberOfLogEntries = 10
            });
            for(var i = 0; i < 20; i++)
            {
                healthCheckLoggerProvider.AddLogEntry(new LogEntry(DateTime.Now, "random", LogLevel.Critical, "random", "random", new EventId(), null));
            }
            healthCheckLoggerProvider.GetLogEntries().Length.Should().Be(10);
        }

        [Fact]
        public async Task LimitWithTimeAsync()
        {
            var healthCheckLoggerProvider = new HealthCheckLoggerProvider(new HealthCheckLoggerProviderConfiguration { 
                FlushTime = TimeSpan.FromMilliseconds(10)
            });
            for (var i = 0; i < 20; i++)
            {
                healthCheckLoggerProvider.AddLogEntry(new LogEntry(DateTime.Now, "random", LogLevel.Critical, "random", "random", new EventId(), null));
            }
            await Task.Delay(50);
            healthCheckLoggerProvider.GetLogEntries().Length.Should().Be(0);
        }


    }
}
