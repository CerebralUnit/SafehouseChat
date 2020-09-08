using Safehouse.Core;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public interface IUserService
    {
        Task<User> GetUser(string id);
        Task<string> Create(User user);
        Task<User> GetUserByUsernameOrEmail(string usernameEmail);
    }
}