using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.SignalR;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;

namespace Safehouse.Chat 
{ 
    public class ChatHub: Hub
    {
        static Dictionary<string, string> userGroup = new Dictionary<string, string>();
        static Dictionary<string, string> userChannel = new Dictionary<string, string>();

        IUserRepository users;
        IChatGroupRepository groups;
        IChatGroupChannelRepository channels;
        IMessageRepository messages;
        public ChatHub(
            IUserRepository userRepository,
            IChatGroupRepository channelRepository,
            IMessageRepository messageRepository,
            IChatGroupChannelRepository chatGroupChannelRepository
            )
        {
            users = userRepository;
            groups = channelRepository;
            messages = messageRepository;
            channels = chatGroupChannelRepository;
        }
        public async Task CreatedMessage(string user, string groupId, string channelId, string message)
        { 
            var messageDetails = new Core.Message()
            {
                Author = await users.RetrieveById(user),
                CreatedAt = DateTime.Now,
                Text = message,
                Channel = channelId,
                ChatGroup = groupId
            };
             
            await Clients.Group(channelId).SendAsync("newMessage", messageDetails);

            var createdMessageId = await messages.Create(messageDetails); 
        }

        public async Task Join(string groupId, string channelId, string userId)
        { 
            await channels.AddParticipant(channelId, userId);

            if (!userGroup.ContainsKey(userId))
                userGroup.Add(userId, channelId);
            else
                userGroup[userId] = channelId;

            if (!userChannel.ContainsKey(userId))
                userChannel.Add(userId, channelId);
            else
                userChannel[userId] = channelId;

            await groups.SetParticipantOnline(groupId, Context.User.Identity.Name);
            await channels.SetParticipantOnline(channelId, Context.User.Identity.Name);

            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        }

        public async Task OnKeydown(string channelId, string userId)
        {
            User user = await users.RetrieveById(userId);
            await Clients.Group(channelId).SendAsync("isTyping", user.Username); 
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
           
            await groups.SetParticipantOffline(userGroup[Context.User.Identity.Name], Context.User.Identity.Name );
            await channels.SetParticipantOffline(userChannel[Context.User.Identity.Name], Context.User.Identity.Name );
            userGroup.Remove(Context.User.Identity.Name);
            userChannel.Remove(Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
