using System;
using System.Collections.Generic;

namespace Safehouse.Core
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; } 

        public string  Password { get; set; }

        public List<string> Channels { get; set; }

        public string ProfilePicture { get; set; }

        public List<string> Friends { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Online { get; set; } 
    }
}
