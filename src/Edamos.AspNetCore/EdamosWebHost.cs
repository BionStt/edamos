using System;
using System.Diagnostics;
using System.IO;
using System.Security.Claims;
using App.Metrics;
using App.Metrics.AspNetCore;
using Community.AspNetCore.ExceptionHandling;
using Edamos.AspNetCore.ExceptionHandling;
using Edamos.AspNetCore.Identity;
using Edamos.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using TraceIdentifiers.AspNetCore;
using TraceIdentifiers.AspNetCore.Serilog;

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
                services.AddEdamosMetrics(config);

                // OPTIONAL: change redis connection if needed
                ConfigurationOptions redisOptions = new ConfigurationOptions ();
                redisOptions.EndPoints.Add(DebugConstants.Redis.DataProtectionHost,
                    DebugConstants.Redis.DataProtectionPort);

                ConnectionMultiplexer redis =
                    ConnectionMultiplexer.Connect(redisOptions, Console.Out);

                //TODO: add cert encryption
                services.AddDataProtection().PersistKeysToRedis(redis).SetApplicationName(AppDomain.CurrentDomain.FriendlyName);                
                
                // TODO: remove if not using SSL termanation
                services.Configure<ForwardedHeadersOptions>(options =>
                {
                    options.ForwardedHeaders = ForwardedHeaders.XForwardedProto;
                });

                services.AddAuthorization(options => options.AddEdamosDefault());

                services.AddMvc().AddJsonOptions(json =>
                {
                    json.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    json.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    json.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                });
                
                services.AddMetrics();
                services.AddExceptionHandlingPolicies(pb => pb.BuildEdamosDefault());
            });
          
            return builder;
        }

        public static IServiceCollection AddEdamosMetrics(this IServiceCollection services, IConfigurationRoot config)
        {
            var metrics = AppMetrics.CreateDefaultBuilder();
            metrics.Report.ToElasticsearch(DebugConstants.ElasticSearch.MetricsUri, "metricsqwe2",
                TimeSpan.FromSeconds(10));
            //metrics.Report.ToConsole(TimeSpan.FromSeconds(10));
            metrics.Configuration.Configure(
                options =>
                {
                    options.DefaultContextLabel = "EDAMOS";
                    options.Enabled = true;
                    options.ReportingEnabled = true;
                });

            services.AddMetrics(metrics);
            
            var hostOptions = new MetricsWebHostOptions();
            
            services.AddMetricsReportScheduler(hostOptions.UnobservedTaskExceptionHandler);
            //services.AddMetricsEndpoints(hostOptions.EndpointOptions);
            services.AddMetricsTrackingMiddleware(hostOptions.TrackingMiddlewareOptions);
            
            return services;
        }

        public static IApplicationBuilder UseEdamosDefaults(this IApplicationBuilder app, IHostingEnvironment env)
        {            
            app.UseForwardedHeaders();            
                 
            app.UseMetricsRequestTrackingMiddleware();
            app.UseTraceIdentifiers().PushToSerilogContext(options => options.AsSeparateProperties("RequestId"));
            return app;
        }

        public static IApplicationBuilder UseEdamosMvcWithAuth(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            return app;
        }

        public static IApplicationBuilder UseEdamosApi(this IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);
            
            // TODO: disable if hosting environment also do it
            app.UseResponseBuffering();
            app.UseCors();
            app.UseExceptionHandlingPolicies();
            
            app.UseAuthentication();

            app.UseMvc();

            return app;
        }

        public static IServiceCollection AddEdamosApiServices(this IServiceCollection services)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.Authority = DebugConstants.IdentityServer.Authority;
                        options.Audience = Consts.Api.ResourceId;
                        options.SaveToken = false;
                        options.Events = new JwtBearerEvents
                        {
                            OnTokenValidated = async vc =>
                            {
                                ClaimsPrincipal principal =
                                    await IdentityHelper.CreateEdamosPrincipal(vc.HttpContext, vc.Principal);
                                vc.HttpContext.User = principal;
                                vc.Principal.AddIdentity(principal.Identity as ClaimsIdentity);
                            }
                        };
                    });
                

            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    // TODO: set correct CORS policy
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });

                options.DefaultPolicyName = "default";
            });            

            return services;
        }
    }
}