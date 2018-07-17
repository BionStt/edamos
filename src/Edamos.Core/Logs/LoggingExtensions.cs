using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Edamos.Core.Logs
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddEdamosLogs(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            LoggerConfiguration conf = new LoggerConfiguration();

            //TODO: use real ELK Uri
            ElasticsearchSinkOptions sinkOptions =
                new ElasticsearchSinkOptions(new Uri(DebugConstants.ElasticSearch.LoggingUri));
            

            configuration.Bind(nameof(ElasticsearchSinkOptions), sinkOptions);

            conf.WriteTo.Elasticsearch(sinkOptions);
            conf.Enrich.FromLogContext();
            conf.Enrich.WithProperty("app", AppDomain.CurrentDomain.FriendlyName);
            conf.Enrich.WithProperty("machine", Environment.MachineName);

            conf.WriteTo.Logger(lc =>
            {
                lc.NewFilterBuilder()
                    .AddSource("Microsoft.EntityFrameworkCore")
                    .RemoveLevelsFromSource("Microsoft.EntityFrameworkCore", LogEventLevel.Debug, LogEventLevel.Verbose)
                    .BuildIncludeOnly();

            }).WriteToRollingFile("efcore.log");

            conf.WriteTo.Logger(lc =>
            {
                lc.NewFilterBuilder()
                    .AddSource("Microsoft.AspNetCore")
                    .RemoveLevelsFromSource("Microsoft.AspNetCore", LogEventLevel.Debug, LogEventLevel.Verbose)
                    .BuildIncludeOnly();

            }).WriteToRollingFile("aspnetcore.log");            

            conf.WriteTo.Logger(lc =>
            {
                lc.NewFilterBuilder()
                    .AddSource("Microsoft.EntityFrameworkCore") // don't log "Microsoft.EntityFrameworkCore" source by default
                    .RemoveLevelsFromSource("Microsoft.EntityFrameworkCore", // keep important events from "Microsoft.EntityFrameworkCore"
                        LogEventLevel.Warning, 
                        LogEventLevel.Error,
                        LogEventLevel.Fatal)

                    .AddSource("Microsoft.AspNetCore") // don't log "Microsoft.AspNetCore" source by default
                    .RemoveLevelsFromSource("Microsoft.AspNetCore", // keep important events from "Microsoft.AspNetCore"
                        LogEventLevel.Warning,
                        LogEventLevel.Error,
                        LogEventLevel.Fatal)
                    .RemoveLevelsFromSource("Microsoft.AspNetCore.DataProtection", // keep important events from "Microsoft.AspNetCore.DataProtection"
                        LogEventLevel.Information,
                        LogEventLevel.Warning,
                        LogEventLevel.Error,
                        LogEventLevel.Fatal)
                    .BuildExclude();

#if DEBUG
                lc.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3} {TraceIdentifier,-30}] {Source}:{EId}:{EName} {NewLine}{Exception}");
#endif
                lc.WriteTo.Elasticsearch(sinkOptions);
            });
                
            Log.Logger = conf.CreateLogger();

            Log.Logger.Information("Startup. {@env}", new Dictionary<string, object>
            {
                {nameof(Environment.UserName), Environment.UserName},
                {nameof(Environment.MachineName), Environment.MachineName},
                {nameof(Environment.CommandLine), Environment.CommandLine},
                {nameof(Environment.CurrentDirectory), Environment.CurrentDirectory},
                {nameof(Environment.StackTrace), Environment.StackTrace},
                {nameof(Environment.SystemDirectory), Environment.SystemDirectory},
                {nameof(Environment.UserDomainName), Environment.UserDomainName},
                {nameof(Environment.OSVersion), Environment.OSVersion.VersionString},
                {nameof(Environment.Is64BitOperatingSystem), Environment.Is64BitOperatingSystem},
                {nameof(Environment.Is64BitProcess), Environment.Is64BitProcess},
                {nameof(Environment.ProcessorCount), Environment.ProcessorCount},
                {nameof(Environment.UserInteractive), Environment.UserInteractive},
            });

            loggingBuilder.AddSerilog(dispose: true);
            return loggingBuilder;
        }
    }
}