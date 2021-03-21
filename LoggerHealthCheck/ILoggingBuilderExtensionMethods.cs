using LoggerHealthCheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.Hosting
{
    public static class ILoggingBuilderExtensionMethods
    {
        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder) => builder.AddHealthCheckLogger<DefaultHealthMessageFormatter>();

        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder, Action<HealthCheckLoggerProviderConfiguration> configure) => builder.AddHealthCheckLogger<DefaultHealthMessageFormatter>(configure);

        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder, HealthCheckLoggerProviderConfiguration configuration) => builder.AddHealthCheckLogger<DefaultHealthMessageFormatter>(configuration);

        public static ILoggingBuilder AddHealthCheckLogger<TMessageFormatter>(this ILoggingBuilder builder) where TMessageFormatter : class, IHealthMessageFormatter => builder.AddHealthCheckLogger<TMessageFormatter>(new HealthCheckLoggerProviderConfiguration());

        public static ILoggingBuilder AddHealthCheckLogger<TMessageFormatter>(this ILoggingBuilder builder, Action<HealthCheckLoggerProviderConfiguration> configure) where TMessageFormatter : class, IHealthMessageFormatter
        {
            var configuration = new HealthCheckLoggerProviderConfiguration();
            configure(configuration);
            return builder.AddHealthCheckLogger<TMessageFormatter>(configuration);
        }

        public static ILoggingBuilder AddHealthCheckLogger<TMessageFormatter>(this ILoggingBuilder builder, HealthCheckLoggerProviderConfiguration configuration) where TMessageFormatter : class, IHealthMessageFormatter
        {
            var provider = new HealthCheckLoggerProvider(configuration);
            builder.AddProvider(provider);
            builder.Services.AddSingleton(provider);
            builder.Services.AddSingleton<IHealthMessageFormatter, TMessageFormatter>();
            return builder;
        }
    }
}
