using System.Reflection;
using Edamos.Core.Users.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Comminity.Extensions.Caching;
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

            services.AddCombinedCache<ApplicationUser>(options =>
            {
                options.MemoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions { SizeLimit = Consts.UsersCache.SizeLimitMemoryBytes }));
                options.DistributedCache = new RedisCache(new RedisCacheOptions
                {
                    InstanceName = nameof(ApplicationUser),
                    Configuration = DebugConstants.Redis.UsersCacheHost
                });
            });

            services.TryAddScoped<IUserManager<ApplicationUser>, EdamosUserManager>();
            services.TryAddScoped<IRoleManager<IdentityRole>, EdamosRoleManager>();

            return services;
        }
    }
}