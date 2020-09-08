using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<List<Message>> RetrieveForChannel(string channelId, int limit = 50, int page = 1);
    }
}