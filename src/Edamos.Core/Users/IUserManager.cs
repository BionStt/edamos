using System.Threading.Tasks;
using Edamos.Core.Cache;

namespace Edamos.Core.Users
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByIdAsync(string userId, CacheUsage cacheUsage);
    }
}