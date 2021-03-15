using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LoggerHealthCheck.Tests
{
    public class ForTypeTests
    {
        ILogger logger;
        HealthCheckService healthCheckService;
        LoggingSource1 loggingSource1;
        LoggingSource2 loggingSource2;

        public ForTypeTests()
        {
            (logger, healthCheckService, loggingSource1, loggingSource2) = ServiceHelper.CreeateServices<ForTypeTests>();
        }

        private async Task<(HealthReportEntry loggingSource1HealthReport, HealthReportEntry loggingSource2HealthReport, HealthReportEntry globalHealthReport)> GetResultAsync()
        {
            var result = await healthCheckService.CheckHealthAsync();
            result.Entries.TryGetValue(IHealthChecksBuilderExtensionMethods.LOGS_NAME, out var globalHealthReport).Should().BeTrue();
            result.Entries.TryGetValue(nameof(LoggingSource1), out var loggingSource1HealthReport).Should().BeTrue();
            result.Entries.TryGetValue(nameof(LoggingSource2), out var loggingSource2HealthReport).Should().BeTrue();
            return (loggingSource1HealthReport, loggingSource2HealthReport, globalHealthReport);
        }

        [InlineData(LogLevel.Trace, HealthStatus.Healthy)]
        [InlineData(LogLevel.Debug, HealthStatus.Healthy)]
        [InlineData(LogLevel.Information, HealthStatus.Healthy)]
        [InlineData(LogLevel.Warning, HealthStatus.Degraded)]
        [InlineData(LogLevel.Error, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Critical, HealthStatus.Unhealthy)]
        [Theory]
        public async Task OnlyLoggingSource1ThrowExceptionAsync(LogLevel logLevel, HealthStatus expectedStatus)
        {
            try
            {
                loggingSource1.ThrowException();
            }
            catch(Exception ex)
            {
                logger.Log(logLevel, ex, "Test");
            }
            var result = await GetResultAsync();
            result.globalHealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource1HealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource2HealthReport.Status.Should().Be(HealthStatus.Healthy);
        }

        [InlineData(LogLevel.Trace, HealthStatus.Healthy)]
        [InlineData(LogLevel.Debug, HealthStatus.Healthy)]
        [InlineData(LogLevel.Information, HealthStatus.Healthy)]
        [InlineData(LogLevel.Warning, HealthStatus.Degraded)]
        [InlineData(LogLevel.Error, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Critical, HealthStatus.Unhealthy)]
        [Theory]
        public async Task OnlyLoggingSource1LogException(LogLevel logLevel, HealthStatus expectedStatus)
        {
            loggingSource1.LogException(logLevel, "Test");
            var result = await GetResultAsync();
            result.globalHealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource1HealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource2HealthReport.Status.Should().Be(HealthStatus.Healthy);
        }


        [InlineData(LogLevel.Trace, HealthStatus.Healthy)]
        [InlineData(LogLevel.Debug, HealthStatus.Healthy)]
        [InlineData(LogLevel.Information, HealthStatus.Healthy)]
        [InlineData(LogLevel.Warning, HealthStatus.Degraded)]
        [InlineData(LogLevel.Error, HealthStatus.Unhealthy)]
        [InlineData(LogLevel.Critical, HealthStatus.Unhealthy)]
        [Theory]
        public async Task OnlyLoggingSource1Log(LogLevel logLevel, HealthStatus expectedStatus)
        {
            loggingSource1.Log(logLevel, "Test");
            var result = await GetResultAsync();
            result.globalHealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource1HealthReport.Status.Should().Be(expectedStatus);
            result.loggingSource2HealthReport.Status.Should().Be(HealthStatus.Healthy);
        }
    }
}
