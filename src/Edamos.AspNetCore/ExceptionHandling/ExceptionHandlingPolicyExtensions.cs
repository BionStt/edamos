using System;
using System.Linq;
using Community.AspNetCore.ExceptionHandling;
using Community.AspNetCore.ExceptionHandling.Mvc;
using Edamos.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;

namespace Edamos.AspNetCore.ExceptionHandling
{
    public static class ExceptionHandlingPolicyExtensions
    {
        public static IExceptionPolicyBuilder BuildEdamosDefault(this IExceptionPolicyBuilder builder)
        {
            builder.For<EdamosApiException>()
                .Log(lo =>
                {
                    lo.EventIdFactory = (c, e) => e.EventId;
                    lo.Level = (c, e) => e.LogLevel;
                    lo.StateFactory = (c, e, o) => new FormattedLogValues(
                        $"{e.Message}. Details: {e.DetailsFormat}", e.DetailsParams.ToArray());

                    lo.Category = (c, e) =>  Consts.Logs.EdamosApiCategory;
                })
                .Response(e => (int)e.StatusCode)
                .ClearCacheHeaders()
                .WithObjectResult(
                    (r, e) => new EdamosApiError
                    {
                        StatusCode = r.HttpContext.Response.StatusCode,
                        Message = e.Message,
                        TraceIdentifier = r.HttpContext.TraceIdentifier
                    })
                .Handled();

            builder.For<Exception>()
                .Log(lo =>
                {
                    lo.Category = (c, e) => Consts.Logs.EdamosApiCategory;
                    lo.Level = (c, e) => LogLevel.Error;
                    lo.EventIdFactory = (c, e) => LogEvents.UnhandledException;
                })
                .Response(null, ResponseAlreadyStartedBehaviour.GoToNextHandler)
                .ClearCacheHeaders()
                .WithObjectResult((r, e) => new EdamosApiError
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