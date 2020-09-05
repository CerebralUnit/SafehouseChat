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
    public class ChatGroupMembershipMySqlRepository : MySQLRepository<ChatGroupMembership>, IChatGroupMembershipRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`chat_group_member`
                                                (`user_id`,
                                                `chat_group_id`,
                                                `online`)
                                                VALUES
                                                (@user_id,
                                                @chat_group_id,
                                                @online);
                                                ";

        const string RETRIEVE_QUERY = @"SELECT * FROM `safehouse`.`chat_group_member` WHERE user_id = @userId AND chat_group_id = @chatGroupId;";

        const string RETRIEVE_ONLINE_QUERY = @"SELECT u.* FROM safehouse.chat_group_channel_member m
                                                JOIN chat_user u ON u.id = m.user_id
                                                WHERE chat_group_channel_id = @chatGroupChannelId;";
        public ChatGroupMembershipMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(ChatGroupMembership obj)
        {
            var group = new Dictionary<string, object>()
            {
                { "@user_id", obj.UserId },
                { "@chat_group_id", obj.ChatGroupId},
                { "@online", obj.Online }, 
            };
             
            return await ExecuteNonQueryGetId(INSERT_STATEMENT, group);
        }
 
        public async Task<bool> Delete(ChatGroupMembership obj)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Exists(string key1, string key2)
        {
            return Retrieve( key1,  key2) != null;
        }

        public async Task<ChatGroupMembership> Retrieve(string key1, string key2)
        {
            ChatGroupMembership membership = null;

            var relationship = new Dictionary<string, object>() {
                { "@userId", key1 },
                { "@chatGroupId", key2 }
            };

            using(var membershipData = await ExecuteQuery(RETRIEVE_QUERY, relationship))
            {
                membership = membershipData.As(x => new ChatGroupMembership() { 
                    ChatGroupId = x.Field<string>("chat_group_id"),
                    UserId = x.Field<Guid>("user_id").ToString(),
                    Online = x.Field<bool>("online")
                }); 
            }
            
            return membership;
        }


        public async Task<List<User>> RetrieveOnlineMembers(string chatGroupChannelId)
        {
            List<User> users = null;

            var channelParams = new Dictionary<string, object>() {
                { "@chatGroupChannelId", chatGroupChannelId }, 
            };

            using (var membershipData = await ExecuteQuery(RETRIEVE_ONLINE_QUERY, channelParams))
            {
                users = membershipData.ToList(x => new User()
                {
                    Username = x.Field<string>("username"),
                    Email = x.Field<string>("email"),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Online = x.Field<bool>("online"),
                    ProfilePicture = x.Field<string>("picture"),
                    Password = x.Field<string>("password"),
                    Id = x.Field<Guid>("id").ToString()
                });
            }

            return users;
        }

        public async Task<bool> Update(ChatGroupMembership obj)
        {
            throw new NotImplementedException();
        }
    }
}
