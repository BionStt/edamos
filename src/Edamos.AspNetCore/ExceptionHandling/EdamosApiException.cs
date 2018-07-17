using System;
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
    }
}