using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using Community.Extensions.Caching.Combined;
using Microsoft.AspNetCore.Identity;

namespace Edamos.Core.Users
{
    public class EdamosUserManager : IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ICombinedCache<ApplicationUser> _cache;

        public EdamosUserManager(
            UserManager<ApplicationUser> userManager,
            ICombinedCache<ApplicationUser> cache)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }        

        public Task<ApplicationUser> FindByIdAsync(string userId, bool allowCache)
        {
            if (allowCache)
            {
                return this._cache.GetOrSetValueAsync(userId, () => this._userManager.FindByIdAsync(userId));
            }

            return this._userManager.FindByIdAsync(userId);
        }

        public Task<IList<string>> GetRolesAsync(ApplicationUser user, bool allowCache)
        {
            if (allowCache)
            {
                return this._cache.GetOrSetValueAsync("r_" + user.Id, () => this._userManager.GetRolesAsync(user));
            }

            return this._userManager.GetRolesAsync(user);
        }
    }
}