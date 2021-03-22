using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace LoggerHealthCheck
{
    public static class Filters
    {
        public const string DefaultHealthCheckServiceFullName = "Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService";
        /// <summary>
        /// Creates a filter that only allows where the Source is the class or a exception with the classname in the stacktrace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<LogEntry, bool> GetFilterForType<T>()
        {
            var fullName = typeof(T).FullName;
            var shortName = typeof(T).Name;
            return (le) => le.Source == fullName || (le.Exception?.StackTrace?.Contains(shortName) ?? false);
        }

        /// <summary>
        /// Creates a filter that only allows where a exception with the classname and method in the stacktrace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Func<LogEntry, bool> GetExceptionFilterForMethod<T>(string methodName)
        {
            var shortName = typeof(T).Name;
            var name = $"{shortName}.{methodName}(";
            return (le) => le.Exception?.StackTrace?.Contains(name) ?? false;
        }

        /// <summary>
        /// Allways returns true
        /// </summary>
        public readonly static Func<LogEntry, bool> DefaultHealthCheck = (_) => true;

        /// <summary>
        /// Ignores log entries from "Microsoft.Extensions.Diagnostics.HealthChecks.DefaultHealthCheckService" that are missing exceptions.
        /// </summary>
        public readonly static Func<LogEntry, bool> DefaultGlobalFilter = (le) => !(le.Source == DefaultHealthCheckServiceFullName && le.Exception == null);

        /// <summary>
        /// Combines two filters, both of them must return true
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Func<LogEntry, bool> Combine(Func<LogEntry, bool> first, Func<LogEntry, bool> second) => (le) => first(le) && second(le);
    }
}
