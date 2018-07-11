using System;
using System.Collections.Generic;
using System.Linq;
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
                ClientId = DebugConstants.KibanaUi.ClientId,
                ClientSecret = DebugConstants.KibanaUi.ClientSecret
            });

            services.AddAuthorization(options =>
            {
                options.AddEdamosDefault();

                options.DefaultPolicy = options.GetPolicy(Policy.LogsName);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);

            app.UseWhen(NotContent, b => b.UseAuthentication().UseAuthorization(Policy.LogsName));            

            app.RunProxy(new ProxyOptions
            {
                Host = Consts.Kibana.Host,
                Port = Consts.Kibana.Port.ToString("D"),
                Scheme = Consts.Kibana.Scheme
            });
        }

        private static bool NotContent(HttpContext context)
        {
            return !context.Request.Path.Value.EndsWith(".css") && 
                   !context.Request.Path.Value.EndsWith(".svg") &&
                   !context.Request.Path.Value.EndsWith(".png") &&
                   !context.Request.Path.Value.EndsWith(".js");
        }        
    }
}
