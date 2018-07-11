using Edamos.AspNetCore.Identity;
using Edamos.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Edamos.AspNetCore
{
    public static class Policy
    {
        public static string LogsName = "Logs";
        public static AuthorizationPolicy Logs { get; } = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
            .RequireRole("logs", "admin").Build();

        public static string AdminName = "Admin";
        public static AuthorizationPolicy Admin { get; } = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
            .RequireRole("admin").Build();

        public static string MetricsName = "Metrics";
        public static AuthorizationPolicy Metrics { get; } = new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
            .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddAuthenticationSchemes(OpenIdConnectDefaults.AuthenticationScheme)
            .RequireRole("admin").Build();

        public static void AddEdamosDefault(this AuthorizationOptions options)
        {
            options.AddPolicy(AdminName, Admin);
            options.AddPolicy(LogsName, Logs);
            options.AddPolicy(MetricsName, Metrics);
        }

        public static IApplicationBuilder AuthorizePath(this IApplicationBuilder app, string path, string policyName)
        {
            app.MapWhen(
                context => context.Request.Path.StartsWithSegments(new PathString(path)),
                b => b.UseAuthentication().UseAuthorization(policyName));

            return app;
        }
    }
}