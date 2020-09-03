using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> RetrieveById(string id);

        Task<bool> SubscribeToChannel(string channelId, string userId);
    }
}
