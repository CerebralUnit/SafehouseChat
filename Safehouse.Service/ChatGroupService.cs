using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;
using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Safehouse.Service
{
    public class ChatGroupService : IChatGroupService
    {
        IChatGroupRepository chatGroups;
        IChatGroupMembershipRepository chatGroupMembers;
        IChatGroupChannelRepository channels;
        IMessageRepository messages; 
        public ChatGroupService(
            IChatGroupRepository chatGroupRepository, 
            IChatGroupMembershipRepository membershipRepository,
            IChatGroupChannelRepository channelRepository,
            IMessageRepository messageRepository 
            )
        {
            chatGroups = chatGroupRepository;
            chatGroupMembers = membershipRepository;
            channels = channelRepository;
            messages = messageRepository; 
        }

        public async Task<string> CreateGroup(ChatGroup group)
        {
            var newGroupId = await chatGroups.Create(group); 

            if(!String.IsNullOrWhiteSpace(newGroupId))
            {
                await CreateChannel(new ChatGroupChannel()
                {
                    CreatedAt = DateTime.Now,
                    Creator = group.Creator,
                    Name = "general",
                    ParentGroup = newGroupId,
                    Type = ChannelType.Text
                });

                chatGroupMembers.Create(new ChatGroupMembership()
                {
                    UserId = group.Creator,
                    ChatGroupId = newGroupId
                });
            }

            return newGroupId;
        }

        public async Task<ChatGroup> GetChatGroup(string chatGroupId)
        {
            var groupTask = chatGroups.Retrieve(chatGroupId);

            var groupChannelsTask = channels.RetrieveByGroup(chatGroupId);

            Task.WaitAll(groupTask, groupChannelsTask);

            var group = groupTask.Result;

            group.Channels = groupChannelsTask.Result;

            return group;
        }

        public async Task<ChatGroupChannel> GetChannelDetails(string channelId)
        {
            var channelsTask = channels.Retrieve(channelId);
            var messagesTask = messages.RetrieveForChannel(channelId);
            var participantsTask = chatGroupMembers.RetrieveOnlineMembers(channelId);

            Task.WaitAll(channelsTask, messagesTask, participantsTask);
             
            var channel = channelsTask.Result;

            channel.Messages = messagesTask.Result;
            channel.Participants = participantsTask.Result;

            return channel; 
        }

        public async Task<ChatGroupChannel> GetChannelDetailsByName(string groupId, string channelName)
        {
            var channel = await channels.RetrieveByName(groupId, channelName);
            var messagesTask = messages.RetrieveForChannel(channel.Id);
            var participantsTask = chatGroupMembers.RetrieveOnlineMembers(channel.Id);

            Task.WaitAll( messagesTask, participantsTask);
  
            channel.Messages = messagesTask.Result;
            channel.Participants = participantsTask.Result;

            return channel;
        }

        public async Task<List<ChatGroup>> GetChatGroups(List<string> chatGroupIds)
        {
            return await chatGroups.RetrieveMany(chatGroupIds);
        }

        public async Task<List<ChatGroup>> GetChatGroupsForUser(string userId)
        {
            return await chatGroups.RetrieveForUser(userId);
        }

        public async Task<string> CreateChannel(ChatGroupChannel channel)
        {
            return await channels.Create(channel);
        }

        public async Task<bool> SubscribeToGroup(string groupId, string userId)
        {
            return await chatGroupMembers.Create(new ChatGroupMembership()
            {
                ChatGroupId =groupId,
                Online = false,
                UserId = userId
            });
        }
    }
}
