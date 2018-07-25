using System.Reflection;
using Community.Extensions.Caching;
using Community.Extensions.Caching.AppMetrics;
using Community.Extensions.Caching.Redis;
using Edamos.Core.Users.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Edamos.Core.Users
{
    public static class ServiceCollectionExtensionsIdentity
    {
        public static IServiceCollection AddEdamosUsers(this IServiceCollection services)
        {
            //TODO: use real connection sting
            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlServer(DebugConstants.ConnectionStrings.IdentityUsersStore,
                    sql => sql.MigrationsAssembly(typeof(UsersDbContext).GetTypeInfo().Assembly.GetName().Name)));

            services.AddIdentityCore<ApplicationUser>().AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<UsersDbContext>();

            services.AddMemoryCache<ApplicationUser>().AddMetricsToMemoryCache<ApplicationUser>();
            services.AddRedisDistributedCache<ApplicationUser>(options =>
            {
                options.Configuration = DebugConstants.Redis.UsersCacheHost;
                options.InstanceName = string.Empty;
            }).AddMetricsToDistributedCache<ApplicationUser>();

            services.AddCombinedCache<ApplicationUser>();           

            services.TryAddScoped<IUserManager<ApplicationUser>, EdamosUserManager>();
            services.TryAddScoped<IRoleManager<IdentityRole>, EdamosRoleManager>();

            return services;
        }
    }
}