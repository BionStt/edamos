using System;
using System.Reflection;
using Edamos.Core.Users.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Edamos.Core;
using Edamos.Core.Cache;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            
            services.TryAddSingleton<IDistributedCache<ApplicationUser>>(
                new DistributedCacheRedis<ApplicationUser>(new RedisCacheOptions
                {
                    InstanceName = nameof(ApplicationUser),
                    Configuration = DebugConstants.Redis.UsersCacheHost
                }));

            services.TryAddScoped<IUserManager<ApplicationUser>, EdamosUserManager>();

            return services;
        }
    }
}