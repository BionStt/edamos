using System;
using System.Runtime.Serialization;

namespace Edamos.AspNetCore.ExceptionHandling
{
    [Serializable]
    [DataContract]
    public class EdamosApiError
    {
        [DataMember(Name = "statusCode")]
        public int StatusCode { get; set; }

        [DataMember(Name = "message")]
        public string Message { get; set; }

        [DataMember(Name = "traceIdentifier")]
        public string TraceIdentifier { get; set; }
    }
}