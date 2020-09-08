using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Safehouse.Repository.AmazonS3;
using Safehouse.Repository.Interfaces; 

namespace SafehouseChat.Controllers
{
    public class Api : Controller
    {
        IMessageRepository messages;

        public Api(IMessageRepository messageRepository)
        {
            messages = messageRepository;
        }

        public async Task<IActionResult> Messages(string channel, int limit = 50, int page = 1)
        {
            var messageList = await messages.RetrieveForChannel(channel, limit, page);
            return Json(messageList);
        }

        public async Task<IActionResult> UploadFiles()
        {
            string imgPath = null;

            if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                imgPath = await (new S3Repository().UploadChatImage(Request.Form.Files[0]));

            return Json(imgPath);
        }
    }
}
