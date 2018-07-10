using System;
using System.Threading.Tasks;
using Edamos.Core.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Edamos.Core.Users
{
    public class EdamosUserManager : DistributedCacheWrapper<EdamosUserManager,ApplicationUser>, IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EdamosUserManager(UserManager<ApplicationUser> userManager,
            IDistributedCache<ApplicationUser> distributedCache) : base(distributedCache)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, CacheUsage cacheUsage)
        {
            ApplicationUser result = await
                this.GetOrSetDistributedCache(cacheUsage, userId, null, async () => await this._userManager.FindByIdAsync(userId));

            return result;
        }        
    }
}