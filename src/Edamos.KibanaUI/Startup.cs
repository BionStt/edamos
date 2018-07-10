using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Edamos.AspNetCore;
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
        const string DefaultPolicy = "default";
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
                    options.Events.OnValidatePrincipal = async context =>
                    {
                        var sp = services.BuildServiceProvider();
                        IUserManager<ApplicationUser> userManager =
                            sp.GetRequiredService<IUserManager<ApplicationUser>>();

                        string userId = context.Principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

                        var claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Role, "logs"));
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
                    
                    options.ClientId = DebugConstants.KibanaUi.ClientId;
                    options.ClientSecret = DebugConstants.KibanaUi.ClientSecret;
                    options.ResponseType = Consts.OpenId.ResponseTypeCodeToken;
                    options.CallbackPath = new PathString(Consts.OpenId.CallbackPath);
                    options.SignedOutCallbackPath = new PathString(Consts.OpenId.SignOutCallbackPath);

                    options.SaveTokens = true;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(DefaultPolicy,
                    builder => builder.RequireAuthenticatedUser()
                        .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
                        .RequireRole("logs", "admin"));

                    options.DefaultPolicy = options.GetPolicy(DefaultPolicy);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseEdamosDefaults(env);

            app.UseWhen(NotContent, b => b.UseAuthentication().Use(Authorize));            

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

        private static async Task Authorize(HttpContext context, Func<Task> next)
        {
            // Use this if there are multiple authentication schemes
            AuthenticateResult authResult = await context.AuthenticateAsync();

            if (authResult.Succeeded && authResult.Principal.Identity.IsAuthenticated)
            {
                IAuthorizationService authorizationService =
                    context.RequestServices.GetRequiredService<IAuthorizationService>();

                AuthorizationResult result =
                    await authorizationService.AuthorizeAsync(authResult.Principal, DefaultPolicy);

                if (result.Succeeded)
                {
                    await next();
                }
                else
                {
                    await context.ForbidAsync();
                }
            }
            else if (authResult.Failure != null)
            {
                // Rethrow, let the exception page handle it.
                ExceptionDispatchInfo.Capture(authResult.Failure).Throw();
            }
            else
            {
                await context.ChallengeAsync();
            }
        }
    }
}
