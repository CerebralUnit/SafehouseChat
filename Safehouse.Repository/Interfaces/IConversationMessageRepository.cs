using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IConversationMessageRepository : IRepository<Message>
    {
        Task<List<Message>> RetrieveForConversation(string conversationId, int limit = 50, int page = 1);
    }
}