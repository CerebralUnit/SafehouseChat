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
    public class ChatGroupChannelMySqlRepository : MySQLRepository<ChatGroupChannel>, IChatGroupChannelRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`chat_group_channel`
                                            ( 
                                            `name`,
                                            `chat_group_id`,
                                            `creator`,
                                            `created_at`,
                                            `type`)
                                            VALUES
                                            ( 
                                            @name,
                                            @chat_group_id,
                                            @creator,
                                            @created_at,
                                            @type);
                                            ";

        const string RETRIEVE_QUERY = @"SELECT * FROM `safehouse`.`chat_group_channel` WHERE id = @id";
        const string RETRIEVE_BY_NAME_QUERY = @"SELECT * FROM `safehouse`.`chat_group_channel` WHERE chat_group_id = @chatGroupId AND name = @name";

        const string RETRIEVE_MANY_QUERY = @"SELECT * FROM `safehouse`.`chat_group_channel` WHERE {0}";

        const string RETRIEVE_BY_GROUP_QUERY = @"SELECT * FROM safehouse.chat_group_channel WHERE chat_group_id = @chatGroupId;";
        const string SET_ONLINE_STATEMENT = @"UPDATE `safehouse`.`chat_group_channel_member`
                                            SET `online` = 1
                                            WHERE `user_id` = @userId AND chat_group_channel_id = @chatGroupChannelId;";

        const string SET_OFFLINE_STATEMENT = @"UPDATE `safehouse`.`chat_group_channel_member`
                                            SET `online` = 0
                                            WHERE `user_id` = @userId AND chat_group_channel_id = @chatGroupChannelId;";

        const string INSERT_PARTICIPANT_STATEMENT = @"INSERT IGNORE INTO `safehouse`.`chat_group_channel_member`
                                                        (
                                                        `user_id`,
                                                        `chat_group_channel_id`,
                                                        `online` 
                                                        )
                                                    VALUES
                                                        (
                                                        @userId,
                                                        @chatGroupChannelId,
                                                        @online
                                                        );";

        const string REMOVE_PARTICIPANT_STATEMENT = @"DELETE FROM `safehouse`.`chat_group_channel_member`
                                                      WHERE user_id = @userId AND chat_group_channel_id = @chatGroupChannelId;";
        public ChatGroupChannelMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(ChatGroupChannel obj)
        {
            var channel = new Dictionary<string, object>()
            {
                { "@name", obj.Name },
                { "@chat_group_id", obj.ParentGroup },
                { "@creator", obj.Creator },
                { "@created_at", obj.CreatedAt },
                { "@type", obj.Type.ToString() } 
            };

            return await ExecuteNonQueryGetId(INSERT_STATEMENT, channel);
        }
        public async Task<bool> AddParticipant(string channelId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chatGroupChannelId", channelId },
                { "@online", true }
            };

            return await ExecuteNonQuery(INSERT_PARTICIPANT_STATEMENT, parameters);
        }


        public async Task<bool> RemoveParticipant(string channelId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chatGroupChannelId", channelId }
            };

            return await ExecuteNonQuery(REMOVE_PARTICIPANT_STATEMENT, parameters);
        }
        public async Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> SetParticipantOnline(string groupId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chatGroupChannelId", groupId }
            };

            return await ExecuteNonQuery(SET_ONLINE_STATEMENT, parameters);
        }


        public async Task<bool> SetParticipantOffline(string groupId, string userId)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "@userId", userId },
                { "@chatGroupChannelId", groupId }
            };

            return await ExecuteNonQuery(SET_OFFLINE_STATEMENT, parameters);
        }
        public async Task<ChatGroupChannel> Retrieve(string id)
        {
            ChatGroupChannel channel = null;

            using(var channelData = await ExecuteQuery(RETRIEVE_QUERY, new Dictionary<string, object>() { { "@id", id } }))
            {
                channel = channelData.As(x => new ChatGroupChannel()
                {
                    Name = x.Field<string>("name"),
                    ParentGroup = x.Field<int>("chat_group_id").ToString(),
                    Creator = x.Field<Guid>("creator").ToString(),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Type = (ChannelType)Enum.Parse(typeof(ChannelType), x.Field<string>("type")),
                    Id = x.Field<int>("id").ToString() 
                });
            }

            return channel;
        }

        public async Task<ChatGroupChannel> RetrieveByName(string groupId, string name)
        {
            ChatGroupChannel channel = null;

            using (var channelData = await ExecuteQuery(RETRIEVE_BY_NAME_QUERY, new Dictionary<string, object>() { { "@name", name }, { "@chatGroupId", groupId } }))
            {
                channel = channelData.As(x => new ChatGroupChannel()
                {
                    Name = x.Field<string>("name"),
                    ParentGroup = x.Field<int>("chat_group_id").ToString(),
                    Creator = x.Field<Guid>("creator").ToString(),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Type = (ChannelType)Enum.Parse(typeof(ChannelType), x.Field<string>("type")),
                    Id = x.Field<int>("id").ToString()
                });
            }

            return channel;
        }

        public async Task<List<ChatGroupChannel>> RetrieveByGroup(string groupId)
        { 
            List<ChatGroupChannel> channels = null;

            using (var channelData = await ExecuteQuery(RETRIEVE_BY_GROUP_QUERY, new Dictionary<string, object>() { { "@chatGroupId", groupId } }))
            {
                channels = channelData.ToList(x => new ChatGroupChannel()
                {
                    Name = x.Field<string>("name"),
                    ParentGroup = x.Field<int>("chat_group_id").ToString(),
                    Creator = x.Field<Guid>("creator").ToString(),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Type = (ChannelType)Enum.Parse(typeof(ChannelType), x.Field<string>("type")),
                    Id = x.Field<int>("id").ToString()
                });
            }

            return channels;
        }

        public async Task<List<ChatGroupChannel>> RetrieveMany(List<string> keys)
        {
            var orQuery = BuildOrQuery("id", "@id", keys);

            var finalQuery = String.Format(RETRIEVE_MANY_QUERY, orQuery.WhereQuery);

            List<ChatGroupChannel> channels = null;

            using (var channelData = await ExecuteQuery(finalQuery, orQuery.Parameters))
            {
                channels = channelData.ToList(x => new ChatGroupChannel()
                {
                    Name = x.Field<string>("name"),
                    ParentGroup = x.Field<int>("chat_group_id").ToString(),
                    Creator = x.Field<Guid>("creator").ToString(),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Type = (ChannelType)Enum.Parse(typeof(ChannelType), x.Field<string>("type")),
                    Id = x.Field<int>("id").ToString()
                });
            }

            return channels;
        }

        public async Task<bool> Update(ChatGroupChannel obj)
        {
            throw new NotImplementedException();
        }
    }
}
