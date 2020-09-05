using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;

namespace Safehouse.Chat.Controllers
{
    public class Group : Controller
    {
        IUserService users;
        IChatGroupService groups;
        
        public Group(IUserService userService, IChatGroupService chatGroupService)
        {
            groups = chatGroupService;
            users = userService;
        }

        public async Task<IActionResult> Index(string id, string channelId)
        {
            var user = await users.GetUser(User.Identity.Name); 
            var thisGroup = await groups.GetChatGroup(id);

            ChatGroupChannel channel = null;

            if (channelId == null)
                channel = await groups.GetChannelDetailsByName(id, "general");
            else
                channel = await groups.GetChannelDetails(channelId);

            ViewData["ChannelId"] = id;
            ViewBag.User = user;
            ViewBag.Channel = channel; 
            ViewBag.Group = thisGroup;

            return View("Chat");
        }

        [HttpPost]
        public async Task<IActionResult> New(ChatGroupModel channel)
        { 
            var newChannelId = await groups.CreateGroup(channel.ToChatGroup());

            if (!String.IsNullOrWhiteSpace(newChannelId)) 
                return Redirect(newChannelId); 

            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Join(string id)
        {
            var thisChannel = await groups.GetChatGroup(id);

            return View(thisChannel);
        }

        [HttpPost]
        public async Task<IActionResult> Subscribe(string id)
        {
            var subscribed = await groups.SubscribeToGroup(id, User.Identity.Name);
            
            if(subscribed != null)
            {
                return Redirect(id);
            }

            return View("Join");
        }
    }
}
