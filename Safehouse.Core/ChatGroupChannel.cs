using System;
using System.Collections.Generic;

namespace Safehouse.Core
{ 
    public enum ChannelType
    {
        Text,
        Voice
    }

    public class ChatGroupChannel
    {
        public string Id { get; set; }

        public List<Message> Messages { get; set; } 

        public string Creator { get; set; }
       
        public List<User> Participants { get; set; } 

        public bool IsCurrent { get; set; }

        public string Name { get; set; }
        
        public DateTime CreatedAt { get; set; }  

        public string ParentGroup { get; set; }  

        public ChannelType Type { get; set; }
    }
}
