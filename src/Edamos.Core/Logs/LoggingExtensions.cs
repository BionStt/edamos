using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace Edamos.Core.Logs
{
    public static class LoggingExtensions
    {
        public static ILoggingBuilder AddEdamosLogs(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
        {
            LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

            //TODO: use real ELK Uri
            ElasticsearchSinkOptions sinkOptions =
                new ElasticsearchSinkOptions(new Uri(DebugConstants.ElasticSearch.LoggingUri));        
            configuration.Bind(nameof(ElasticsearchSinkOptions), sinkOptions);

            loggerConfiguration.WriteTo.Elasticsearch(sinkOptions);

#if DEBUG
            loggerConfiguration.WriteTo.Console();
#endif
            Log.Logger = loggerConfiguration.CreateLogger();

            loggingBuilder.AddSerilog(dispose: true);
            return loggingBuilder;
        }
    }
}