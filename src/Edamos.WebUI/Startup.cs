using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Edamos.AspNetCore;
using Edamos.Core;
using Edamos.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Events.OnSigningIn = context => { return Task.CompletedTask; };
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        var sp = services.BuildServiceProvider();
                        IUserManager<ApplicationUser> userManager =
                            sp.GetRequiredService<IUserManager<ApplicationUser>>();
                        UserManager<ApplicationUser> um;
                        string userId = context.Principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

                        var claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Role, "qwe"));
                        ApplicationUser user = await userManager.FindByIdAsync(userId);

                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                            ClaimTypes.Name, ClaimTypes.Role);

                        context.ReplacePrincipal(new ClaimsPrincipal(identity));
                        
                    };
                })
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = DebugConstants.IdentityServer.Authority;

                    options.ClientId = DebugConstants.Ui.ClientId;
                    options.ClientSecret = DebugConstants.Ui.ClientSecret;
                    options.ResponseType = Consts.OpenId.ResponseTypeCodeToken;
                    options.CallbackPath = new PathString(Consts.OpenId.CallbackPath);
                    options.SignedOutCallbackPath = new PathString(Consts.OpenId.SignOutCallbackPath);

                    options.SaveTokens = false;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();
        }
    }
}
