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
        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder) => builder.AddHealthCheckLogger(new HealthCheckLoggerProviderConfiguration());

        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder, Action<HealthCheckLoggerProviderConfiguration> configure)
        {
            var configuration = new HealthCheckLoggerProviderConfiguration();
            configure(configuration);
            return builder.AddHealthCheckLogger(configuration);
        }

        public static ILoggingBuilder AddHealthCheckLogger(this ILoggingBuilder builder, HealthCheckLoggerProviderConfiguration configuration)
        {
            var provider = new HealthCheckLoggerProvider(configuration);
            builder.AddProvider(provider);
            builder.Services.AddSingleton(provider);
            return builder;
        }
    }
}
