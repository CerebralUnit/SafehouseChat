using MongoDB.Bson;
using MongoDB.Driver;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Repository.MySql
{
    public class FriendRequestMySqlRepository : MySQLRepository<FriendRequest>, IFriendRequestRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`friend_request`
                                            (`sender_id`,
                                            `recipient_id`,
                                            `status`,
                                            `sent_at` )
                                            VALUES
                                            (@sender_id,
                                            @recipient_id,
                                            @status,
                                            @sent_at);
                                            ";

        const string UPDATE_STATEMENT = @"UPDATE `safehouse`.`friend_request`
                                            SET 
                                            `status` = @status, 
                                            `accepted_at` = @accepted_at
                                            WHERE `sender_id` = @senderId AND `recipient_id` = @recipientId;
";

        const string RETRIEVE_PENDING_QUERY = @"SELECT * FROM `safehouse`.`friend_request` WHERE (sender_id = @userId OR recipient_id = @userId) AND status = 'Pending'";

        public FriendRequestMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<bool> Create(FriendRequest obj)
        {
            var request = new Dictionary<string, object>()
            {
                { "@sender_id", obj.Sender },
                { "@recipient_id", obj.Recipient},
                { "@status", obj.Status },
                { "@sent_at", obj.SentAt } 
            };

            try
            {
                await ExecuteNonQuery(INSERT_STATEMENT, request);
            }
            catch { return false; }

            return true;
        }

        public async Task<bool> Delete(FriendRequest obj)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exists(string key1, string key2)
        {
            throw new NotImplementedException();
        }

        public async Task<FriendRequest> Retrieve(string key1, string key2)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FriendRequest>> RetrievePendingRequests(string userId)
        {
            var requests = new List<FriendRequest>();

            using (var requestData = await ExecuteQuery(RETRIEVE_PENDING_QUERY, new Dictionary<string, object>() { { "@userId", userId } }))
            {
                requests = requestData.ToList(x => new FriendRequest()
                {
                    Sender = x.Field<Guid>("sender_id").ToString(),
                    Recipient = x.Field<Guid>("recipient_id").ToString(),
                    SentAt = x.Field<DateTime>("sent_at"),
                    Status = (FriendRequestStatus)Enum.Parse(typeof(FriendRequestStatus), x.Field<string>("status"))
                });
            }

            return requests;
        }

        public async Task<bool> Update(FriendRequest obj)
        {
           var request = new Dictionary<string, object>() {
                { "@status", obj.Status.ToString() },
                { "@accepted_at", obj.AcceptedAt },
                { "@senderId", obj.Sender },
                { "@recipientId", obj.Recipient }
            };

            try
            {
                await ExecuteNonQuery(UPDATE_STATEMENT, request);
            }
            catch { return false; }

            return true;
        }
    }
}
