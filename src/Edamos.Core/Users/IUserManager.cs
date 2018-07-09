using System.Threading.Tasks;

namespace Edamos.Core.Users
{
    public interface IUserManager<TUser>
    {
        Task<TUser> FindByIdAsync(string userId);
    }
}