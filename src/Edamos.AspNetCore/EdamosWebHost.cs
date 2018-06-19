using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Edamos.AspNetCore
{
    public static class WebHostExtensions
    {       
        public static IWebHostBuilder AddEdamosDefault(this IWebHostBuilder builder, string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var currentDir = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            builder.UseConfiguration(config);

            return builder;
        }
    }
}