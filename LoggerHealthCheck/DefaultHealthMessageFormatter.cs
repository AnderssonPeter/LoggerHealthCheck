using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;

namespace LoggerHealthCheck
{
    public class DefaultHealthMessageFormatter : IHealthMessageFormatter
    {
        public HealthMessage GenerateMessage(LogEntry[] entries, LogLevel unhealthyLogLevel, TimeSpan flushTime)
        {
            StringBuilder builder;
            HealthStatus status = HealthStatus.Degraded;
            builder = new StringBuilder();
            var grouped = entries.GroupBy(e => new { e.LogLevel, e.Source, MessageTemplate = e.MessageTemplate ?? e.Message, ExceptionType = e.Exception?.GetType(), ExceptionMessage = e.Exception?.ToString() }, e => new { e.Timestamp, e.Message })
                                 .Select(x => new { x.Key, latestOccurence = x.Max(x => x.Timestamp), numberOfOccurrences = x.Count(), messages = x.Select(x => x.Message) })
                                 .OrderByDescending(x => x.latestOccurence);
            foreach (var group in grouped)
            {
                var key = group.Key;
                if (group.Key.LogLevel >= unhealthyLogLevel)
                {
                    status = HealthStatus.Unhealthy;
                }
                var values = group;
                builder.AppendLine($"{key.LogLevel}: {key.Source} - {key.MessageTemplate}");
                var messages = values.messages.Distinct().Where(x => x != key.MessageTemplate).ToArray();
                const int numberOfMessages = 3;
                foreach (var message in messages.Take(numberOfMessages))
                {
                    builder.AppendLine(message);
                }

                var count = messages.Count();
                if (count > numberOfMessages)
                {
                    builder.Append("And ");
                    builder.Append(count - numberOfMessages);
                    builder.AppendLine(" more");
                }


                builder.AppendLine($"Latest occurrence: {values.latestOccurence}, number of occurrences: {values.numberOfOccurrences} in the last {flushTime.ToHumanReadableString()}");
                if (key.ExceptionType != null)
                {
                    builder.AppendLine($"{key.ExceptionType}: {key.ExceptionMessage}");
                }
                builder.AppendLine();
            }
            
            return new HealthMessage(status, builder.ToString().TrimEnd(Environment.NewLine.ToCharArray()));
        }
    }
}
