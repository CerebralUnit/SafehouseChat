﻿using Safehouse.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Safehouse.Repository.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<List<Conversation>> RetrieveUserConversations(string userId);
    }
}
