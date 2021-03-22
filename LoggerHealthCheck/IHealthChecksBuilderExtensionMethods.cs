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

        /// <summary>
        /// Creates a healthcheck that only checks where the Source is the class or a exception with the classname in the stacktrace
        /// </summary>
        /// <returns></returns>
        public static IHealthChecksBuilder AddLoggerHealthCheckForType<T>(this IHealthChecksBuilder builder,
            string? name = default, Action<LoggerHealthCheckOptions>? setup = default,
            IEnumerable<string>? tags = default, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            var typeFilter = Filters.GetFilterForType<T>();
            return AddLoggerHealthCheckForType<T>(builder, name, setup, tags, failureStatus, typeFilter);
        }

        /// <summary>
        /// Creates a healthcheck that changes status when a exception occures that has the specified function in its stacktrace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHealthChecksBuilder AddExceptionLoggerHealthCheckForMethod<T>(this IHealthChecksBuilder builder, string methodName, 
            string? name = default, Action<LoggerHealthCheckOptions>? setup = default, 
            IEnumerable<string>? tags = default, HealthStatus failureStatus = HealthStatus.Unhealthy)
        {
            var typeFilter = Filters.GetExceptionFilterForMethod<T>(methodName);
            return AddLoggerHealthCheckForType<T>(builder, name, setup, tags, failureStatus, typeFilter);
        }

        /// <summary>
        /// Creates a healthcheck that with the specified filter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IHealthChecksBuilder AddLoggerHealthCheckForType<T>(IHealthChecksBuilder builder, string? name, Action<LoggerHealthCheckOptions>? setup, IEnumerable<string>? tags, HealthStatus failureStatus, Func<LogEntry, bool> typeFilter)
        {
            var options = new LoggerHealthCheckOptions();
            setup?.Invoke(options);
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
