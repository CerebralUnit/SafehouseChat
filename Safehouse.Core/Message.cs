using System;

namespace Safehouse.Core
{
    public class Message
    {
       public string Text { get; set; }

       public string ChatGroup { get; set; }
        public string Channel { get; set; }
       
       public User Author { get; set; }

       public DateTime CreatedAt { get; set; } 
    }
}
