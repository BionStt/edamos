using Comminity.Extensions.Caching;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edamos.Core.Users
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByIdAsync(string userId, AllowedCaches cacheUsage);

        Task<IList<string>> GetRolesAsync(TUser user, AllowedCaches cacheUsage);
    }
}