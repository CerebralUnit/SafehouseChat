using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public class UserService : IUserService
    {
        IUserRepository users;
        IChatGroupService chatGroups;
        IChatGroupMembershipRepository chatGroupMembership;
        IFriendRequestRepository friendRequests;
        IConversationRepository conversations;
        public UserService(
           IUserRepository userRepository,
           IChatGroupMembershipRepository chatGroupMembershipRepository,
           IChatGroupService chatGroupService,
           IFriendRequestRepository friendRequestRepository,
           IConversationRepository conversationRepository)
        {
            users = userRepository;
            chatGroups = chatGroupService;
            chatGroupMembership = chatGroupMembershipRepository;
            friendRequests = friendRequestRepository;
            conversations = conversationRepository;
        }

        public async Task<User> GetUser(string id)
        {
            var userTask = users.RetrieveById(id);
            var groupsTask = chatGroups.GetChatGroupsForUser(id);

            Task.WaitAll(userTask, groupsTask);

            User user = userTask.Result;

            if(user != null)
                user.ChatGroups = groupsTask.Result;
             
            return user;
        }

        public async Task<User> GetUserByUsernameOrEmail(string usernameEmail)
        {
            var user = await users.Retrieve(usernameEmail);
            var groups = await chatGroups.GetChatGroupsForUser(user.Id);
              
            user.ChatGroups = groups;

            return user;
        }

        public async Task<string> Create(User user)
        { 
            return await users.Create(user);
        }

        public async Task<bool> SendFriendRequest(string fromUserId, string toUsernameOrEmail)
        {
            User user = await GetUserByUsernameOrEmail(toUsernameOrEmail);
            bool created = false;

            if(user != null)
            {
                created = await friendRequests.Create(new FriendRequest()
                {
                    SentAt = DateTime.Now,
                    Recipient = user.Id,
                    Sender = fromUserId,
                    Status = FriendRequestStatus.Pending
                });
            }
            else
            {
                throw new Exception("User does not exist");
            }

            return created; 
        }
        
        public async Task<List<FriendRequest>> GetPendingFriendRequests(string userId)
        {
            var requests = await friendRequests.RetrievePendingRequests(userId);

            var sentRequests = requests.Where(x => x.Sender == userId);
            var receivedRequests = requests.Where(x => x.Recipient == userId);

            var userIds = new List<string>(sentRequests.Select(x => x.Recipient));

            userIds.AddRange(receivedRequests.Select(x => x.Sender));

            var userList = await users.RetrieveMany(userIds.Distinct().ToList());

            foreach(var request in sentRequests)
            {
                request.OtherUser = userList.FirstOrDefault(x => x.Id == request.Recipient);
            }

            foreach (var request in receivedRequests)
            {
                request.OtherUser = userList.FirstOrDefault(x => x.Id == request.Sender);
            }

            return requests;
        }

        public async Task<List<User>> GetFriends(string userId)
        {
            return await users.RetrieveFriends(userId);
        }

        public async Task<string> CreateConversation(string creatorId, List<string> userIds)
        {
            var ids = new List<string>(userIds);

            ids.Add(creatorId);
 
            var list = String.Join('-', ids.OrderBy(x => x).ToArray()).Replace("-", "");

            var hash = CreateMD5(list);

            await conversations.Create(new Conversation()
            {
                Id = hash,
                Members = ids,
                CreatedBy = creatorId,
                CreatedAt = DateTime.Now
            });
             
            return hash; 
        }

        public async Task<List<Conversation>> GetConversations(string userId)
        {
            return await conversations.RetrieveUserConversations(userId);
        }

        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
