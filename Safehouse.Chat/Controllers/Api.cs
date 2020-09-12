using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Integration;
using Safehouse.Repository.AmazonS3;
using Safehouse.Repository.Interfaces;
using Safehouse.Service;

namespace SafehouseChat.Controllers
{
    public class Api : Controller
    {
        IMessageRepository messages;
        IUserService users;
        IConversationMessageRepository conversationMessages;
        public Api(
            IMessageRepository messageRepository,
            IConversationMessageRepository conversationMessageRepository,
            IUserService userService
            )
        {
            messages = messageRepository;
            users = userService;
            conversationMessages = conversationMessageRepository;
        }

        public async Task<IActionResult> Messages(string channel, int limit = 50, int page = 1)
        {
            var messageList = await messages.RetrieveForChannel(channel, limit, page);
            return Json(messageList);
        }

        public async Task<IActionResult> ConversationMessages(string id, int limit = 50, int page = 1)
        {
            var messageList = await conversationMessages.RetrieveForConversation(id, limit, page);
            return Json(messageList);
        }

        public async Task<IActionResult> UploadFiles()
        {
            string imgPath = null;

            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                imgPath = await (new S3Repository().UploadChatImage(Request.Form.Files[0]));

            return Json(imgPath);
        }

        public async Task<IActionResult> TenorTrending()
        {
            var trending = await (new TenorGifClient().Trending());

            return Json(trending);
        }


        public async Task<IActionResult> TenorSearch(string q)
        {
            var results = await (new TenorGifClient().Search(q));

            return Json(results);
        }

        public async Task<IActionResult> TenorCategories()
        {
            var results = await (new TenorGifClient().Categories());

            return Json(results);
            
        }

        public async Task<IActionResult> FriendRequest(string usernameOrEmail)
        {
            var succeeded = await users.SendFriendRequest(User.Identity.Name, usernameOrEmail);
            return Json(succeeded);

        }

        public async Task<IActionResult> Friends()
        {
            var friends = await users.GetFriends(User.Identity.Name);

            return Json(friends);
        }

        public async Task<IActionResult> CreateConversation(List<string> userIds)
        {
            var conversationId = await users.CreateConversation(User.Identity.Name, userIds);
            return Json(conversationId);
        } 

        public async Task<IActionResult> GetConversations()
        {
            var convos = await users.GetConversations(User.Identity.Name);

            return Json(convos);
        }
    }
}
