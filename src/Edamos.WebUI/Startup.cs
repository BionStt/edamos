using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edamos.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Edamos.WebUI
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
            services.AddAuthentication("oidc") .AddOpenIdConnect("oidc", options =>
            {
                options.SignInScheme = "Cookies";

                options.Authority = DebugConstants.IdentityServer.Authority;

                options.ClientId = DebugConstants.Ui.ClientId;
                options.ClientSecret = DebugConstants.Ui.ClientSecret;
                options.ResponseType = "code id_token";

                options.SaveTokens = true;
                //options.GetClaimsFromUserInfoEndpoint = true;

                options.Scope.Add("openid");
                options.Scope.Add("offline_access");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
