using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IChatGroupMembershipRepository : IRelationshipRepository<ChatGroupMembership>
    {
        Task<List<User>> RetrieveOnlineMembers(string chatGroupChannelId);
    }
}
