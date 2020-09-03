using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Repository.Interfaces;

namespace Safehouse.Chat.Controllers
{
    public class Channel : Controller
    {
        IUserRepository users;
        IChannelRepository channels;
        public Channel(IUserRepository userRepository, IChannelRepository channelRepository)
        {
            channels = channelRepository;
            users = userRepository;
        }
        public async Task<IActionResult> Index(string id)
        {
            var user = await users.RetrieveById(User.Identity.Name);
            var userChannels = await channels.RetrieveMany(user.Channels); 
            var thisChannel = await channels.Retrieve(id);

        
            if (thisChannel.Messages == null || thisChannel.Messages.Count == 0)
                thisChannel.Messages = new List<Core.Message>() { new Core.Message() { Author = new Core.User() { Username = "Chat Bot" }, Text = $"Welcome to {thisChannel.Name}", CreatedAt = DateTime.Now } };
            ViewData["ChannelId"] = id;
            ViewBag.User = user;
            ViewBag.Channel = thisChannel;
            ViewBag.Channels = userChannels;
            return View("Chat");
        }

        [HttpPost]
        public async Task<IActionResult> New(ChannelModel channel)
        {
            var newChannel = new Core.Channel()
            {
                Name = channel.ChannelName,
                Picture = channel.ChannelPicture ?? "",
                Creator = User.Identity.Name,
                ParticipantIds = new List<string>()
                {
                    User.Identity.Name
                }
            };
            
            var newChannelId = await channels.Create(newChannel);

            if (!String.IsNullOrWhiteSpace(newChannelId))
            {
                return Redirect("channel/" + newChannelId);
            }

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Join(string id)
        {
            var thisChannel = await channels.Retrieve(id);

            return View(thisChannel);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string id)
        {
            var subscribed = await users.SubscribeToChannel(id, User.Identity.Name);
            
            if(subscribed)
            {
                return Redirect("/channel/" + id);
            }

            return View("Join");
        }
    }
}
