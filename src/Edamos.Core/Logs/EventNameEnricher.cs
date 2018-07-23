
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
                            Truncate(sv.Properties[1].Value?.ToString()));

                        logEvent.AddPropertyIfAbsent(ename);
                    }
                    else
                    {
                        var ename = propertyFactory.CreateProperty(
                            EventNamePropertyName,
                            Truncate(logEvent.MessageTemplate?.Text));

                        logEvent.AddPropertyIfAbsent(ename);
                    }
                }
            }
            else
            {
                var eid = propertyFactory.CreateProperty(EventIdPropertyName, -1);
                logEvent.AddPropertyIfAbsent(eid);
                var ename = propertyFactory.CreateProperty(EventNamePropertyName,
                    Truncate(logEvent.MessageTemplate?.Text));
                logEvent.AddPropertyIfAbsent(ename);
            }
        }

        private static string Truncate(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            var startsWith = value.IndexOf('{');
            if (startsWith == 0 && value.Length > 2)
            {
                int endIndex = value.IndexOf(':');

                if (endIndex < 2)
                {
                    endIndex = value.IndexOf('}');
                }

                if (endIndex > 1)
                {
                    value = value.Substring(startsWith, endIndex + 1);
                }                
            }

            if (value.Length > 100)
            {
                value = value.Substring(0, 100);
            }

            return value;
        }
    }
}