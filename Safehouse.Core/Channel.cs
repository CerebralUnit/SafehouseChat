using System;
using System.Collections.Generic;

namespace Safehouse.Core
{
    public class Channel
    {
        public string Id { get; set; }

        public List<Message> Messages { get; set; }
        public List<string> MessageIds { get; set; }

        public string Creator { get; set; }
       
        public List<User> Participants { get; set; }
        public List<string> ParticipantIds { get; set; }

        public string Name { get; set; }
        
        public DateTime CreatedAt { get; set; } 
        
        public string Picture { get; set; }
    }
}
