using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IChatGroupRepository : IRepository<ChatGroup>
    {
        Task<bool> AddParticipant(string groupId, string userId);
        Task<bool> RemoveParticipant(string groupId, string userId);
        Task<bool> AddMessage(string groupId, string userId, string message);
        Task<List<ChatGroup>> RetrieveForUser(string userId);
        Task<bool> SetParticipantOnline(string groupId, string userId);
        Task<bool> SetParticipantOffline(string groupId, string userId);
    }
}
