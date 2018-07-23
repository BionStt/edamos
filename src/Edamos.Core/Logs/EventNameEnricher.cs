
using System;
using System.Linq;
using Serilog.Core;
using Serilog.Events;

namespace Edamos.Core.Logs
{
    public class EventNameEnricher : ILogEventEnricher
    {
        public const string EventIdPropertyName = "EId";

        public const string EventNamePropertyName = "EName";

        /// <summary>
        /// The enrich.
        /// </summary>
        /// <param name="logEvent">
        /// The log event.
        /// </param>
        /// <param name="propertyFactory">
        /// The property factory.
        /// </param>
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {            
            if (logEvent.Properties.TryGetValue("EventId", out var value))
            {
                if (value is StructureValue sv)
                {
                    if (sv.Properties.Count > 0 && int.TryParse(sv.Properties[0].Value.ToString(), out var v))
                    {
                        var eid = propertyFactory.CreateProperty(EventIdPropertyName, v);
                        logEvent.AddPropertyIfAbsent(eid);
                    }

                    if (sv.Properties.Count > 1)
                    {
                        var ename = propertyFactory.CreateProperty(
                            EventNamePropertyName,
                            Truncate(logEvent, sv.Properties[1].Value?.ToString()));

                        logEvent.AddPropertyIfAbsent(ename);
                    }
                    else
                    {
                        var ename = propertyFactory.CreateProperty(
                            EventNamePropertyName,
                            Truncate(null, logEvent.MessageTemplate?.Text));

                        logEvent.AddPropertyIfAbsent(ename);
                    }
                }
            }
            else
            {
                var eid = propertyFactory.CreateProperty(EventIdPropertyName, -1);
                logEvent.AddPropertyIfAbsent(eid);
                var ename = propertyFactory.CreateProperty(EventNamePropertyName,
                    Truncate(null, logEvent.MessageTemplate?.Text));
                logEvent.AddPropertyIfAbsent(ename);
            }
        }

        private static string Truncate(LogEvent logEvent, string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            ReadOnlySpan<char> span = value;
            span = span.Trim('"');

            if (logEvent != null)
            {
                if (logEvent.Properties.TryGetValue("SourceContext", out var context))
                {
                    ReadOnlySpan<char> oldValue = context.ToString();
                    oldValue = oldValue.Trim('"');

                    if (span.StartsWith(oldValue) && span.Length > oldValue.Length + 1)
                    {
                        span = span.Slice(oldValue.Length + 1);
                    } 
                }
            }

            if (span.StartsWith("{") && span.Length > 2)
            {
                int endIndex = span.IndexOf(':');

                if (endIndex < 2)
                {
                    endIndex = span.IndexOf('}');
                }

                if (endIndex > 1)
                {
                    span = span.Slice(1, endIndex - 1);
                }                
            }
            else if (span.Length > 10)
            {
                int endIndex = span.IndexOf('{');

                if (endIndex > 10)
                {
                    span = span.Slice(0, endIndex);
                }
            }

            if (span.Length > 100)
            {
                span = span.Slice(0, 100);
            }

            return span.Trim().ToString();
        }
    }
}