using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerHealthCheck.Tests
{
    class ServiceHelper
    {
        public static (ILogger logger, HealthCheckService healthCheckService, LoggingSource1 loggingSource1, LoggingSource2 loggingSource2) CreeateServices<T>()
        {
            var services = new ServiceCollection();
            services.AddLogging(builder => builder.AddHealthCheckLogger());
            services.AddSingleton<LoggingSource1>();
            services.AddSingleton<LoggingSource2>();
            services.AddHealthChecks()
                    .AddLoggerHealthCheckForType<LoggingSource1>()
                    .AddLoggerHealthCheckForType<LoggingSource2>()
                    .AddLoggerHealthCheck();
            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<T>>();
            var healthCheckService = serviceProvider.GetRequiredService<HealthCheckService>();
            var loggingSource1 = serviceProvider.GetRequiredService<LoggingSource1>();
            var loggingSource2 = serviceProvider.GetRequiredService<LoggingSource2>();

            return (logger, healthCheckService, loggingSource1, loggingSource2);
        }
    }
}
