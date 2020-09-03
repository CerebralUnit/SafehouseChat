using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IChannelRepository : IRepository<Channel>
    {
        Task<bool> AddParticipant(string channelId, string userId);
        Task<bool> RemoveParticipant(string channelId, string userId);
        Task<bool> AddMessage(string channelId, string userId, string message);
    }
}
