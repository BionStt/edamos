using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Comminity.Extensions.Caching;
using Edamos.Core.Cache;
using Microsoft.AspNetCore.Identity;

namespace Edamos.Core.Users
{
    public class EdamosUserManager : IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ICombinedCache<ApplicationUser> _cache;

        public EdamosUserManager(
            UserManager<ApplicationUser> userManager,
            ICombinedCache<ApplicationUser> cache,
            IMetrics metrics)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }        

        public Task<ApplicationUser> FindByIdAsync(string userId, AllowedCaches allowedCache)
        {
            return this._cache.GetOrAddAsync(allowedCache, userId, () => this._userManager.FindByIdAsync(userId));
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, AllowedCaches allowedCache)
        {
            return this._cache.GetOrAddAsync(allowedCache, "r_" + user.Id, () => this._userManager.GetRolesAsync(user));            
        }
    }
}