using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.SignalR;
using Safehouse.Repository.Interfaces;

namespace Safehouse.Chat 
{ 
    public class ChatHub: Hub
    {
        static Dictionary<string, string> userChannel = new Dictionary<string, string>();

        IUserRepository users;
        IChatGroupRepository channels;
        IMessageRepository messages;
        public ChatHub(
            IUserRepository userRepository,
            IChatGroupRepository channelRepository,
            IMessageRepository messageRepository
            )
        {
            users = userRepository;
            channels = channelRepository;
            messages = messageRepository;
        }
        public async Task CreatedMessage(string user, string channelId, string message)
        { 
            var messageDetails = new Core.Message()
            {
                Author = await users.RetrieveById(user),
                CreatedAt = DateTime.Now,
                Text = message
            };
             
            await Clients.Group(channelId).SendAsync("newMessage", messageDetails);

            var createdMessageId = await messages.Create(messageDetails);

            await channels.AddMessage(channelId, user, createdMessageId);
        }

        public async Task Join(string channelId, string userId)
        {
            await channels.AddParticipant(channelId, userId);
            userChannel.Add(userId, channelId);
            await Groups.AddToGroupAsync(Context.ConnectionId, channelId);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
           
            await channels.RemoveParticipant(userChannel[Context.User.Identity.Name], Context.User.Identity.Name );
            userChannel.Remove(Context.User.Identity.Name);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
