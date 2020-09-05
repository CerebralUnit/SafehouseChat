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

        const string RETRIEVE_QUERY = @"SELECT * FROM `safehouse`.`chat_group` WHERE id = @id;";

        const string RETRIEVE_MANY_QUERY = @"SELECT * FROM `safehouse`.`chat_group` WHERE {0}";

        const string RETRIEVE_FOR_USER_QUERY = @"SELECT g.* FROM safehouse.chat_group_member m
                                                JOIN safehouse.chat_group g ON g.id = m.chat_group_id
                                                WHERE user_id = @userId;";

        public ChatGroupMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(ChatGroup obj)
        { 
            var group = new Dictionary<string, object>()
            {
                { "@group_icon", obj.Picture },
                { "@name", obj.Name },
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


        public async Task<ChatGroup> Retrieve(string id)
        {
            ChatGroup channel = null;

            using (var groupData = await ExecuteQuery(RETRIEVE_QUERY, new Dictionary<string, object>() { { "@id", int.Parse(id) } }))
            {
                channel = groupData.As(x => new ChatGroup()
                {
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Creator = x.Field<Guid>("creator").ToString(),
                    Name = x.Field<string>("name"),
                    Picture = x.Field<string>("group_icon"),
                    Id = x.Field<int>("id").ToString()
                });
            }
             
            return channel;
        }

        public async Task<List<ChatGroup>> RetrieveForUser(string userId)
        {
            List<ChatGroup> groups = null;

            using (var groupData = await ExecuteQuery(RETRIEVE_FOR_USER_QUERY, new Dictionary<string, object>() { { "@userId", userId } }))
            {
                groups = groupData.ToList(x => new ChatGroup()
                {
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Creator = x.Field<Guid>("creator").ToString(),
                    Name = x.Field<string>("name"),
                    Picture = x.Field<string>("group_icon"),
                    Id = x.Field<int>("id").ToString()
                });
            }

            return groups;
        }


        public async Task<List<ChatGroup>> RetrieveMany(List<string> keys)
        {
            var groups = new List<ChatGroup>();

            if (keys == null || keys.Count == 0)
                return groups;

            var orQuery = BuildOrQuery("id", "@id", keys);

            var finalQuery = String.Format(RETRIEVE_MANY_QUERY, orQuery.WhereQuery);
             
            using (var groupsData = await ExecuteQuery(finalQuery, orQuery.Parameters))
            {
                groups = groupsData.ToList(x => new ChatGroup()
                {
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Creator = x.Field<Guid>("creator").ToString(),
                    Name = x.Field<string>("name"),
                    Picture = x.Field<string>("group_icon"),
                    Id = x.Field<int>("id").ToString(),
                });
            }
 
            return groups;
        }

        

        public Task<bool> Update(ChatGroup obj)
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
