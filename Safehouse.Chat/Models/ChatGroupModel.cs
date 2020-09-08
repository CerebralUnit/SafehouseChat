using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Chat
{
    public class ChatGroupModel
    { 
        public string GroupId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelPicture { get; set; }
        public string CreatorId { get; set; }

        public ChatGroup ToChatGroup()
        {
            return new ChatGroup()
            {
                Name = this.ChannelName,
                Picture = this.ChannelPicture ?? "",
                Creator = CreatorId
            };
        }

        public ChatGroupChannel ToChatGroupChannel()
        {
            return new ChatGroupChannel()
            {
                ParentGroup = GroupId,
                Name = this.ChannelName, 
                Creator = CreatorId
            };
        }
    }
}
