using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public interface IChatGroupService
    {
        Task<string> CreateGroup(ChatGroup group);
        Task<ChatGroup> GetChatGroup(string chatGroupId);
        Task<List<ChatGroup>> GetChatGroups(List<string> chatGroupIds);
        Task<List<ChatGroup>> GetChatGroupsForUser(string userId);
        Task<string> CreateChannel(ChatGroupChannel channel);
        Task<ChatGroupChannel> GetChannelDetails(string channelId);
        Task<ChatGroupChannel> GetChannelDetailsByName(string groupId, string channelName);
        Task<string> SubscribeToGroup(string groupId, string userId);
    }
}
