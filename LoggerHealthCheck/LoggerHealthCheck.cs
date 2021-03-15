using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LoggerHealthCheck
{
    public class LoggerHealthCheck : IHealthCheck
    {
        private readonly HealthCheckLoggerProvider healthCheckLoggerProvider;
        private readonly LoggerHealthCheckOptions options;

        public LoggerHealthCheck(HealthCheckLoggerProvider healthCheckLoggerProvider, LoggerHealthCheckOptions options)
        {
            this.healthCheckLoggerProvider = healthCheckLoggerProvider;
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
            var status = HealthStatus.Degraded;
            StringBuilder builder = new StringBuilder();
            var grouped = entries.GroupBy(e => new { e.LogLevel, e.Source, e.Message, ExceptionType = e.Exception?.GetType(), ExceptionMessage = e.Exception?.ToString() }, e => e.Timestamp);
            foreach(var group in grouped)
            {                
                var key = group.Key;
                if (group.Key.LogLevel >= options.UnhealthyLogLevel)
                {
                    status = HealthStatus.Unhealthy;
                }
                var values = new { latestOccurence = group.Max(), numberOfOccurrences = group.Count() };
                builder.AppendLine($"{key.LogLevel}: {key.Source} - {key.Message}");
                builder.AppendLine($"Latest occurrence: {values.latestOccurence}, number of occurrences: {values.numberOfOccurrences} in the last {healthCheckLoggerProvider.Configuration.FlushTime.ToHumanReadableString()}");
                if (key.ExceptionType != null)
                {
                    builder.AppendLine($"{key.ExceptionType}: {key.ExceptionMessage}");
                }
                builder.AppendLine();
            }

            return Task.FromResult(new HealthCheckResult(status, builder.ToString()));
        }
    }
}
