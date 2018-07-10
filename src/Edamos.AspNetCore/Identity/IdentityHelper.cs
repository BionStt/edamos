using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Threading.Tasks;
using Edamos.Core;
using Edamos.Core.Cache;
using Edamos.Core.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Edamos.AspNetCore
{
    public static class IdentityHelper
    {
        public static async Task<ClaimsPrincipal> CreateEdamosPrincipal(HttpContext context, ClaimsPrincipal principal)
        {
            IUserManager<ApplicationUser> userManager =
                context.RequestServices.GetRequiredService<IUserManager<ApplicationUser>>();

            string userId = principal.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.Role, "logs"));
            ApplicationUser user = await userManager.FindByIdAsync(userId, CacheUsage.All);

            claims.Add(new Claim(ClaimTypes.Name, user.UserName));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name, ClaimTypes.Role);

            return new ClaimsPrincipal(identity);
        }

        public static async Task SignInEdamosCookie(CookieValidatePrincipalContext context)
        {
            context.ReplacePrincipal(await CreateEdamosPrincipal(context.HttpContext, context.Principal));
        }

        public static IServiceCollection AddEdamosCookieOpenId(this IServiceCollection services, OpenIdCookieSettings settings)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.Events.OnValidatePrincipal = IdentityHelper.SignInEdamosCookie;
                })
                .AddOpenIdConnect(options =>
                {
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                    options.Authority = DebugConstants.IdentityServer.Authority;

                    options.ClientId = settings.ClientId;
                    options.ClientSecret = settings.ClientSecret;
                    options.ResponseType = Consts.OpenId.ResponseTypeCodeToken;
                    options.CallbackPath = new PathString(Consts.OpenId.CallbackPath);
                    options.SignedOutCallbackPath = new PathString(Consts.OpenId.SignOutCallbackPath);

                    options.SaveTokens = settings.SaveTokens;
                });

            return services;
        }

        public static IApplicationBuilder UseAuthorization(this IApplicationBuilder app, string policyName)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (policyName == null) throw new ArgumentNullException(nameof(policyName));

            return app.Use((context, func) => Authorize(context, func, policyName));
        }
        private static async Task Authorize(HttpContext context, Func<Task> next, string policyName)
        {
            // Use this if there are multiple authentication schemes
            AuthenticateResult authResult = await context.AuthenticateAsync();

            if (authResult.Succeeded && authResult.Principal.Identity.IsAuthenticated)
            {
                IAuthorizationService authorizationService =
                    context.RequestServices.GetRequiredService<IAuthorizationService>();

                AuthorizationResult result =
                    await authorizationService.AuthorizeAsync(authResult.Principal, policyName);

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