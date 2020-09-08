using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Core;
using Safehouse.Repository.AmazonS3;
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

            var userChatGroup = user.ChatGroups?.FirstOrDefault(x => x.Id == thisGroup.Id);

            userChatGroup.IsCurrent = true;

            var groupChannel = thisGroup.Channels.FirstOrDefault(x => x.Id == channel.Id);
            groupChannel.IsCurrent = true;

            ViewData["ChannelId"] = channel.Id;
            ViewData["GroupId"] = thisGroup.Id;
            ViewBag.User = user;
            ViewBag.Channel = channel; 
            ViewBag.Group = thisGroup;

            return View("Chat");
        }

        [HttpPost]
        public async Task<IActionResult> New(ChatGroupModel channel)
        {
            string imgPath = null;

            if(Request.Form.Files != null && Request.Form.Files.Count > 0 ) 
                imgPath = await (new S3Repository().UploadImage(Request.Form.Files[0])); 

            ChatGroup group = channel.ToChatGroup();

            group.Picture = imgPath;

            var newChannelId = await groups.CreateGroup(group);

            if (!String.IsNullOrWhiteSpace(newChannelId)) 
                return Redirect(newChannelId); 

            return View();
        }
       

     
        [HttpPost]
        public async Task<IActionResult> NewChannel(ChatGroupModel channel)
        {
            var newChannelId = await groups.CreateChannel(channel.ToChatGroupChannel());

            if (!String.IsNullOrWhiteSpace(newChannelId))
                return Redirect($"/group/{channel.GroupId}/{newChannelId}");

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
