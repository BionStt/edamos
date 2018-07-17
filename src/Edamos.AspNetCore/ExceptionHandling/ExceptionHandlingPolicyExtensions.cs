using Commmunity.AspNetCore.ExceptionHandling;
using System;
using System.Net;
using Commmunity.AspNetCore.ExceptionHandling.Mvc;
using Edamos.AspNetCore.ExceptionHandling;
using Microsoft.Extensions.Logging;

namespace Edamos.AspNetCore
{
    public static class ExceptionHandlingPolicyExtensions
    {
        public static IExceptionPolicyBuilder BuildEdamosDefault(this IExceptionPolicyBuilder builder)
        {
            builder.For<EdamosApiException>()
                .Log(lo =>
                {
                    lo.EventIdFactory = (c, e) =>
                    {
                        EventId eventId = LogEvents.UnhandledException;
                        EdamosApiException ee = e as EdamosApiException;
                        if (ee != null)
                        {
                            eventId = ee.EventId;
                        }

                        return eventId;
                    };

                    lo.Category = (c, e) => LogEvents.EdamosCategory;
                })
                .Response(e => (int)e.StatusCode)
                .ClearCacheHeaders()
                .WithObjectResult(
                    (r, e) => new EdamosApiError()
                    {
                        StatusCode = r.HttpContext.Response.StatusCode,
                        Message = e.Message,
                        TraceIdentifier = r.HttpContext.TraceIdentifier
                    })
                .Handled();

            builder.For<Exception>()
                .Log(lo =>
                {
                    lo.Category = (c, e) => LogEvents.EdamosCategory;
                    lo.Level = (c, e) => LogLevel.Error;
                    lo.EventIdFactory = (c, e) => LogEvents.UnhandledException;
                })
                .Response(null, ResponseAlreadyStartedBehaviour.GoToNextHandler)
                .ClearCacheHeaders()
                .WithObjectResult((r, e) => new EdamosApiError()
                {
                    StatusCode = r.HttpContext.Response.StatusCode,
                    Message = "Internal Server Error",
                    TraceIdentifier = r.HttpContext.TraceIdentifier
                })
                .Handled();

            return builder;
        }
    }
}