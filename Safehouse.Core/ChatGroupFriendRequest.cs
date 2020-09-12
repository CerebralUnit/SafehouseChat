using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;

namespace Safehouse.Core
{
    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Denied
    }

    public class FriendRequest
    {
        public string Sender { get; set; }
        public string Recipient { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? AcceptedAt { get; set; }

        public User OtherUser { get; set; }
    }
}
