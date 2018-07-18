using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Edamos.AspNetCore;
using Edamos.AspNetCore.Identity;
using Edamos.Core;
using Edamos.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edamos.KibanaUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddEdamosCookieOpenId(new OpenIdCookieSettings
            {
                ClientId = DebugConstants.ProxyUi.ClientId,
                ClientSecret = DebugConstants.ProxyUi.ClientSecret
            });

            services.AddAuthorization(options =>
            {
                options.AddEdamosDefault();

                //options.DefaultPolicy = options.GetPolicy(Policy.LogsName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);

            app.MapWhen(c => IsHost(c, Consts.Kibana.Host),
                b =>
                {
                    b.UseWhen(NotContent, nc => nc.UseAuthentication().UseAuthorization(Policy.LogsName));
                    b.RunProxy(new ProxyOptions
                    {
                        Host = Consts.Kibana.Host,
                        Port = Consts.Kibana.Port.ToString("D"),
                        Scheme = Consts.Kibana.Scheme
                    });
                });

            app.MapWhen(c => IsHost(c, Consts.RabbitMq.Host),
                b =>
                {
                    b.UseWhen(NotContent, nc => nc.UseAuthentication().UseAuthorization(Policy.RabbitMqName));
                    b.RunProxy(new ProxyOptions
                    {
                        Host = Consts.RabbitMq.Host,
                        Port = Consts.RabbitMq.Port.ToString("D"),
                        Scheme = Consts.RabbitMq.Scheme
                    });
                });

            app.MapWhen(c => IsHost(c, Consts.Grafana.Host),
                b =>
                {
                    b.UseWhen(NotContent, nc => nc.UseAuthentication().UseAuthorization(Policy.GrafanaName));
                    b.RunProxy(new ProxyOptions
                    {
                        Host = Consts.Grafana.Host,
                        Port = Consts.Grafana.Port.ToString("D"),
                        Scheme = Consts.Grafana.Scheme
                    });
                });

            app.Use((context, func) =>
            {
                context.Response.StatusCode = 400;
                return Task.CompletedTask;
            });
        }

        private static bool IsHost(HttpContext context, string host)
        {
            return context.Request.Host.HasValue && context.Request.Host.Value.StartsWith(host);
        }

        private static bool NotContent(HttpContext context)
        {
            bool notContent = !context.Request.Path.Value.EndsWith(".css") && 
                             !context.Request.Path.Value.EndsWith(".svg") &&
                             !context.Request.Path.Value.EndsWith(".png") &&
                             !context.Request.Path.Value.EndsWith(".js");

            return notContent;
        }        
    }
}
