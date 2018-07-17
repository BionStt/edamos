using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;

namespace Edamos.AspNetCore.ExceptionHandling
{
    [Serializable]
    public class EdamosApiException : Exception
    {
        private const string StatusCodeKey = "7E39A3DEE4D3";
        private const string EventIdKey = "8E39A3DEE4D3";
        private const string LevelKey = "9E39A3DEE4D3";

        public EdamosApiException() : base("Internal Server Error")
        {
        }

        public EdamosApiException(string message) : base(message)
        {
        }
        
        public EdamosApiException(string message, Exception inner) : base(message, inner)
        {
        }
        
        protected EdamosApiException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                if (this.Data.Contains(StatusCodeKey))
                {
                    return (HttpStatusCode) this.Data[StatusCodeKey];
                }

                return HttpStatusCode.InternalServerError;
            }
            set => this.Data[StatusCodeKey] = value;
        }

        public LogLevel LogLevel
        {
            get
            {
                if (this.Data.Contains(LevelKey))
                {
                    return (LogLevel)this.Data[LevelKey];
                }

                return LogLevel.Error;
            }
            set => this.Data[LevelKey] = value;
        }

        public EventId EventId {
            get
            {
                if (this.Data.Contains(EventIdKey))
                {
                    return (EventId)this.Data[EventIdKey];
                }

                return LogEvents.UnhandledException;
            }
            set => this.Data[EventIdKey] = value;
        }

        public EdamosApiException With(string key, object value)
        {
            string k = "{" + key + "}";

            if (!this.Data.Contains(k))
            {
                this.Data.Add(k, value);
            }

            return this;
        }

        public string DetailsFormat => string.Join(' ',
            EnumerateDetailKeys(this.Data.Keys));

        public IEnumerable<object> DetailsParams
        {
            get
            {
                foreach (string key in EnumerateDetailKeys(this.Data.Keys))                
                {
                    yield return this.Data[key];
                }
            }
        }

        private static IEnumerable<string> EnumerateDetailKeys(IEnumerable keys)
        {
            return keys.OfType<string>().Where(s => s.StartsWith("{") && s.EndsWith("}"));
        }
    }
}