using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Edamos.Core.Logs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Edamos.AspNetCore
{
    public static class WebHostExtensions
    {
        private const int DataProtectionRedisDatabase = 1993774;

        public static IWebHostBuilder AddEdamosDefault(this IWebHostBuilder builder, string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var currentDir = Directory.GetCurrentDirectory();

#if DEBUG
            File.Copy("/app/bin/debug/netcoreapp2.0/root.crt", "/usr/local/share/ca-certificates/EdamosRootCA.crt");

            Process.Start("update-ca-certificates")?.WaitForExit(10000);
#endif            
            var config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            builder.UseConfiguration(config);

            builder.ConfigureServices(services =>
            {
                // OPTIONAL: change redis connection if needed
                ConfigurationOptions redisOptions = new ConfigurationOptions ();
                redisOptions.EndPoints.Add("redisdp", 6379);

                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(redisOptions, Console.Out);

                //TODO: add cert encryption
                services.AddDataProtection().PersistKeysToRedis(redis).SetApplicationName(AppDomain.CurrentDomain.FriendlyName);
                services.AddMvc();
                services.AddLogging(logsBuilder => logsBuilder.AddEdamosLogs(config));
            });

#if DEBUG
            builder.Configure(app => app.UseDeveloperExceptionPage());
#endif
            return builder;
        }
    }
}