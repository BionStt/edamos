using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Edamos.Core.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Edamos.Core.Users
{
    public class EdamosUserManager : DistributedCacheWrapper<EdamosUserManager,ApplicationUser>, IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EdamosUserManager(
            UserManager<ApplicationUser> userManager,
            IDistributedCache<ApplicationUser> distributedCache,
            IMetrics metrics) : base(distributedCache, metrics)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CacheUsage cacheUsage)
        {
            ApplicationUser result = await
                this.GetOrSetDistributedCache(cacheUsage, userId, null, async () => await this._userManager.FindByIdAsync(userId));

            return result;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CacheUsage cacheUsage)
        {
            IList<string> result = await this.GetOrSetDistributedCache(cacheUsage, "r_" + user.SecurityStamp, null,
                async () => (await this._userManager.GetRolesAsync(user)).ToList());

            return result;
        }
    }
}