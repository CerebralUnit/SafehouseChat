﻿using System;
using System.Collections.Generic;

namespace Safehouse.Core
{
    public class ChatGroup
    {
        public string Id { get; set; }

        public List<Message> Messages { get; set; } 

        public string Creator { get; set; }
       
        public List<User> Participants { get; set; } 

        public string Name { get; set; }
        
        public DateTime CreatedAt { get; set; } 
        
        public string Picture { get; set; }

        public bool IsCurrent { get; set; }
         
        public List<ChatGroupChannel> Channels { get; set; }
    }
}
