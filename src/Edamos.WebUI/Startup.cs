using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Edamos.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie()
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = DebugConstants.IdentityServer.Authority;

                    options.ClientId = DebugConstants.Ui.ClientId;
                    options.ClientSecret = DebugConstants.Ui.ClientSecret;
                    options.ResponseType = "code id_token";
                    options.CallbackPath = new PathString(Consts.OpenId.CallbackPath);
                    options.SignedOutCallbackPath = new PathString(Consts.OpenId.SignOutCallbackPath);

                    options.SaveTokens = true;
                //options.GetClaimsFromUserInfoEndpoint = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
