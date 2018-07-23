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
            LoggerConfiguration conf = new LoggerConfiguration().ApplyEdamosConfiguration(configuration);
                
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