using Safehouse.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public interface IUserService
    {
        Task<User> GetUser(string id);
        Task<string> Create(User user);
        Task<User> GetUserByUsernameOrEmail(string usernameEmail);

        Task<bool> SendFriendRequest(string fromUserId, string toUserId);
        Task<List<FriendRequest>> GetPendingFriendRequests(string userId);
        Task<List<User>> GetFriends(string userId);
        Task<string> CreateConversation(string creatorId, List<string> userIds);
        Task<List<Conversation>> GetConversations(string userId);
    }
}