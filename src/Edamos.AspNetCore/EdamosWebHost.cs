using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Edamos.Core;
using Edamos.Core.Logs;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Edamos.AspNetCore
{
    public static class WebHostExtensions
    {
        public static IWebHostBuilder AddEdamosDefault(this IWebHostBuilder builder, string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var currentDir = Directory.GetCurrentDirectory();

#if DEBUG
            const string crtInstallPath = "/usr/local/share/ca-certificates/EdamosRootCA.crt";
            if (!File.Exists(crtInstallPath))
            {
                File.Copy("/app/bin/debug/netcoreapp2.0/root.crt", crtInstallPath);

                Process.Start("update-ca-certificates")?.WaitForExit(10000);
            }            
#endif            
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            builder.UseConfiguration(config);

            builder.ConfigureServices(services =>
            {
                services.AddEdamosDefault(config);

                // OPTIONAL: change redis connection if needed
                ConfigurationOptions redisOptions = new ConfigurationOptions ();
                redisOptions.EndPoints.Add(DebugConstants.Redis.DataProtectionHost,
                    DebugConstants.Redis.DataProtectionPort);

                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(redisOptions, Console.Out);

                //TODO: add cert encryption
                services.AddDataProtection().PersistKeysToRedis(redis).SetApplicationName(AppDomain.CurrentDomain.FriendlyName);
                services.AddMvc();
                
                // TODO: remove if not using SSL termanation
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                });
            });
          
            return builder;
        }

        public static IApplicationBuilder UseEdamosDefaults(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders();

#if DEBUG
            app.UseDeveloperExceptionPage();
#endif  
            return app;
        }
    }
}