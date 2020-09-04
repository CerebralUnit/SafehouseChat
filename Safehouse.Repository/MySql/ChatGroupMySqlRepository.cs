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
    public class ChatGroupMySqlRepository : MySQLRepository<ChatGroup>, IChatGroupRepository
    {
        const string INSERT_GROUP_STATEMENT = @"INSERT INTO `safehouse`.`chat_group`
                                            ( 
                                            `group_icon`,
                                            `name`,
                                            `created_at`,
                                            `creator`
                                            )
                                            VALUES
                                            ( 
                                            @group_icon,
                                            @name,
                                            @created_at,
                                            @creator
                                            );";

        const string INSERT_PARTICIPANT_STATEMENT = @"INSERT IGNORE INTO `safehouse`.`chat_group_member`
                                                        (
                                                        `user_id`,
                                                        `chat_group_id` 
                                                        )
                                                    VALUES
                                                        (
                                                        @userId,
                                                        @chat_group_id 
                                                        );";

        const string REMOVE_PARTICIPANT_STATEMENT = @"DELETE FROM `safehouse`.`chat_group_member`
                                                      WHERE user_id = @userId AND chat_group_id = @chatGroupId;";

        const string RETRIEVE_QUERY = @"SELECT * FROM `safehouse`.`chat_group_channel` WHERE id = @id;";

        const string RETRIEVE_MANY_QUERY = @"SELECT * FROM `safehouse`.`chat_group_channel` WHERE {0}";

        public ChatGroupMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(Channel obj)
        { 
            var group = new Dictionary<string, object>()
            {
                { "@group_icon", obj.Name },
                { "@name", obj.Picture },
                { "@created_at", DateTime.Now },
                { "@creator", obj.Creator }
            };
             
            return await ExecuteNonQueryGetId(INSERT_GROUP_STATEMENT, group);
        }

        public Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
       
        public async Task<bool> AddParticipant(string groupId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chat_group_id", groupId } 
            };

            return await ExecuteNonQuery(INSERT_PARTICIPANT_STATEMENT, parameters);
        }


        public async Task<bool> RemoveParticipant(string groupId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chat_group_id", groupId }
            };

            return await ExecuteNonQuery(REMOVE_PARTICIPANT_STATEMENT, parameters);
        }


        public async Task<Channel> Retrieve(string id)
        {
            Channel channel = null;

            using (var channelData = await ExecuteQuery(RETRIEVE_QUERY, new Dictionary<string, object>() { { "@id", id } }))
            {
                channel = channelData.As(x => new Channel()
                {
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Creator = x.Field<string>("creator"),
                    Name = x.Field<string>("name"),
                    Picture = x.Field<string>("group_icon"),
                    Id = x.Field<string>("id")
                });
            }
             
            return channel;
        }

        public async Task<List<Channel>> RetrieveMany(List<string> keys)
        {
            var orQuery = BuildOrQuery("id", "@id", keys);

            var finalQuery = String.Format(RETRIEVE_MANY_QUERY, orQuery.WhereQuery);

            var groups = new List<Channel>();

            using (var groupsData = await ExecuteQuery(finalQuery, orQuery.Parameters))
            {
                groups = groupsData.ToList(x => new Channel()
                {
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Creator = x.Field<string>("creator"),
                    Name = x.Field<string>("name"),
                    Picture = x.Field<string>("group_icon"),
                    Id = x.Field<string>("id"),
                });
            }
 
            return groups;
        }

        

        public Task<bool> Update(Channel obj)
        {
            throw new NotImplementedException();

        }

        public async Task<bool> AddMessage(string channelId, string userId, string messageId)
        { 
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(channelId));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");

            var update = Builders<BsonDocument>.Update.Push("message", new BsonObjectId(messageId));

            var results = await collection.UpdateOneAsync(
                filter,
                update
            );

            return results.IsAcknowledged; 
        }
    }
}
