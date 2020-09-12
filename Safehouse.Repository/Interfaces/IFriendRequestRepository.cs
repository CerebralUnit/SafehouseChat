using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IFriendRequestRepository : IRelationshipRepository<FriendRequest>
    {
        Task<List<FriendRequest>> RetrievePendingRequests(string userId); 
    }
}
