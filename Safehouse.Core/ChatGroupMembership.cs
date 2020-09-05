using System;
using System.Collections.Generic;

namespace Safehouse.Core
{
    public class ChatGroupMembership
    {
        public string UserId { get; set; }

        public string ChatGroupId { get; set; }
 
        
        public bool Online { get; set; }
    }
}
