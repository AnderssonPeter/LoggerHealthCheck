using LoggerHealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class IHealthChecksBuilderExtensionMethods
    {
        public const string LOGS_NAME = "Logs";

        public static IHealthChecksBuilder AddLoggerHealthCheck(this IHealthChecksBuilder builder,
            string? name = default, Action<LoggerHealthCheckOptions>? setup = default,  
            IEnumerable<string>? tags = default, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            var options = new LoggerHealthCheckOptions();
            setup?.Invoke(options);

            return builder.Add(new HealthCheckRegistration(
                name ?? LOGS_NAME,
                sp => new LoggerHealthCheck.LoggerHealthCheck(sp.GetRequiredService<HealthCheckLoggerProvider>(), sp.GetRequiredService<IHealthMessageFormatter>(), options),
                failureStatus,
                tags));
        }


        public static IHealthChecksBuilder AddLoggerHealthCheckForType<T>(this IHealthChecksBuilder builder,
            string? name = default, Action<LoggerHealthCheckOptions>? setup = default,
            IEnumerable<string>? tags = default, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            var options = new LoggerHealthCheckOptions();
            setup?.Invoke(options);
            var typeFilter = Filters.GetFilterForType<T>();
            if (options.Filter != Filters.DefaultHealthCheck)
            {
                var customFilter = options.Filter;
                options.Filter = Filters.Combine(customFilter, typeFilter);
            }
            else
            {
                options.Filter = typeFilter;
            }
            return builder.Add(new HealthCheckRegistration(
                name ?? typeof(T).Name,
                sp => new LoggerHealthCheck.LoggerHealthCheck(sp.GetRequiredService<HealthCheckLoggerProvider>(), sp.GetRequiredService<IHealthMessageFormatter>(), options),
                failureStatus,
                tags));
        }
    }
}
