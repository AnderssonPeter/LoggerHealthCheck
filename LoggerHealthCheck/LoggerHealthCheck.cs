using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerHealthCheck
{
    public class LoggerHealthCheck : IHealthCheck
    {
        private readonly HealthCheckLoggerProvider healthCheckLoggerProvider;
        private readonly IHealthMessageFormatter healthMessageFormatter;
        private readonly LoggerHealthCheckOptions options;

        public LoggerHealthCheck(HealthCheckLoggerProvider healthCheckLoggerProvider, IHealthMessageFormatter healthMessageFormatter, LoggerHealthCheckOptions options)
        {
            this.healthCheckLoggerProvider = healthCheckLoggerProvider;
            this.healthMessageFormatter = healthMessageFormatter;
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var entries = healthCheckLoggerProvider.GetLogEntries().Where(options.Filter).ToArray();
            if (entries.Length == 0)
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }
            //Create message!
            var message = healthMessageFormatter.GenerateMessage(entries, options.UnhealthyLogLevel, healthCheckLoggerProvider.Configuration.FlushTime);

            return Task.FromResult(new HealthCheckResult(message.Status, message.Content));
        }
    }
}
