using Safehouse.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IChatGroupChannelRepository : IRepository<ChatGroupChannel>
    {
        Task<List<ChatGroupChannel>> RetrieveByGroup(string groupId);
        Task<ChatGroupChannel> RetrieveByName(string groupId, string name);
    }
}