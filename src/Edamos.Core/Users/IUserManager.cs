
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edamos.Core.Users
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByIdAsync(string userId, bool allowCache);

        Task<IList<string>> GetRolesAsync(TUser user, bool allowCache);
    }
}