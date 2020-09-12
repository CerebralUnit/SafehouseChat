using System;
using System.Collections.Generic;
using System.Text;

namespace Safehouse.Core
{
    public class Conversation
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public List<string> Members { get; set; } 
        public List<User> MemberUsers { get; set; }
    }
}
