using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace LoggerHealthCheck
{
    public record HealthMessage(HealthStatus Status, string Content);
}
