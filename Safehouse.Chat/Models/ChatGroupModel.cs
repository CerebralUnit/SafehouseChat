using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Chat
{
    public class ChatGroupModel
    {
        public string ChannelName { get; set; }
        public string ChannelPicture { get; set; }
        public string CreatorId { get; set; }

        public ChatGroup ToChatGroup()
        {
            return new Core.ChatGroup()
            {
                Name = this.ChannelName,
                Picture = this.ChannelPicture ?? "",
                Creator = CreatorId
            };
        } 
    }
}
