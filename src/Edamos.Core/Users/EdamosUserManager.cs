using System;
using System.Threading.Tasks;
using Edamos.Core.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;

namespace Edamos.Core.Users
{
    public class EdamosUserManager : IUserManager<ApplicationUser>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IDistributedCache _distributedCache;

        public EdamosUserManager(UserManager<ApplicationUser> userManager, IDistributedCache<ApplicationUser> distributedCache)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
        }

        public Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return this._userManager.FindByIdAsync(userId);
        }
    }
}