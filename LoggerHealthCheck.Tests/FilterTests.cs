using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LoggerHealthCheck.Tests
{
    public class FilterTests
    {
        [InlineData(Filters.DefaultHealthCheckServiceFullName, false, false)]
        [InlineData(Filters.DefaultHealthCheckServiceFullName, true, true)]
        [InlineData("random", false, true)]
        [InlineData("random", true, true)]
        [Theory]
        public void DefaultGlobalFilter(string name, bool hasException, bool expectedResult)
        {
            Filters.DefaultGlobalFilter(new LogEntry(DateTime.Now, name, LogLevel.Trace, "", new EventId(), hasException ? new Exception() : null)).Should().Be(expectedResult);
        }


        [InlineData("random", false, true)]
        [InlineData("random", true, true)]
        [Theory]
        public void DefaultHealthCheck(string name, bool hasException, bool expectedResult)
        {
            Filters.DefaultHealthCheck(new LogEntry(DateTime.Now, name, LogLevel.Trace, "", new EventId(), hasException ? new Exception() : null)).Should().Be(expectedResult);
        }

        [InlineData("LoggerHealthCheck.Tests.LoggingSource1", null, true)]
        [InlineData("LoggerHealthCheck.Tests.LoggingSource1", "random", true)]
        [InlineData("random", "random", true)]
        [InlineData("random", null, false)]
        [Theory]
        public void GetFilterForType(string name, string exceptionMessage, bool expectedResult)
        {
            if (string.IsNullOrEmpty(exceptionMessage))
            {
                Filters.GetFilterForType<LoggingSource1>()(new LogEntry(DateTime.Now, name, LogLevel.Trace, "", new EventId(), null)).Should().Be(expectedResult);
            }
            else
            {
                var loggingSource1 = new LoggingSource1(null);
                try
                {
                    loggingSource1.ThrowException();
                }
                catch (Exception ex)
                {
                    Filters.GetFilterForType<LoggingSource1>()(new LogEntry(DateTime.Now, name, LogLevel.Trace, "", new EventId(), ex)).Should().Be(expectedResult);
                }
            }
        }

        [Fact]
        public void CombineFilters()
        {
            var logEntry = new LogEntry(DateTime.Now, "", LogLevel.Trace, "", new EventId(), null);
            Filters.Combine((_) => true, (_) => true)(logEntry).Should().BeTrue();
            Filters.Combine((_) => false, (_) => true)(logEntry).Should().BeFalse();
            Filters.Combine((_) => false, (_) => false)(logEntry).Should().BeFalse();
            Filters.Combine((_) => true, (_) => false)(logEntry).Should().BeFalse();
        }
    }
}
