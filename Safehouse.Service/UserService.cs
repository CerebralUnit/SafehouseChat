using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public class UserService : IUserService
    {
        IUserRepository users;
        IChatGroupService chatGroups;
        IChatGroupMembershipRepository chatGroupMembership;
        public UserService(
           IUserRepository userRepository,
           IChatGroupMembershipRepository chatGroupMembershipRepository,
           IChatGroupService chatGroupService)
        {
            users = userRepository;
            chatGroups = chatGroupService;
            chatGroupMembership = chatGroupMembershipRepository;
        }

        public async Task<User> GetUser(string id)
        {
            var userTask = users.RetrieveById(id);
            var groupsTask = chatGroups.GetChatGroupsForUser(id);

            Task.WaitAll(userTask, groupsTask);

            User user = userTask.Result;
            user.ChatGroups = groupsTask.Result;
             
            return user;
        }

        public async Task<string> Create(User user)
        { 
            return await users.Create(user);
        }
    }
}
