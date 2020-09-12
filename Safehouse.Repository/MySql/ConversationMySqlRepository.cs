using MongoDB.Bson;
using MongoDB.Driver;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Repository.MySql
{
    public class ConversationMySqlRepository : MySQLRepository<Conversation>, IConversationRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`conversation`
                                        (`id`,
                                        `created_at`,
                                        `created_by`)
                                        VALUES
                                        (
                                        @id,
                                        @created_at,
                                        @created_by);
                                        ";

        const string INSERT_MEMBERS_STATEMENT = @"INSERT INTO `safehouse`.`conversation_member`
                                                (
                                                `conversation_id`,
                                                `user_id`
                                                )
                                                VALUES
                                                {0}";

        const string RETRIEVE_USER_CONVERSATIONS_QUERY = @"SELECT c.id, c.created_at, c.created_by, u.id AS user_id, u.email, u.online, u.username, u.picture, u.created_at AS user_created_at
                                                            FROM safehouse.conversation_member m 
                                                            JOIN safehouse.conversation c ON c.id = m.conversation_id
                                                            JOIN safehouse.conversation_member o ON o.conversation_id = c.id
                                                            JOIN safehouse.chat_user u on u.id = o.user_id
                                                            WHERE m.user_id =   @userId;";

        public ConversationMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(Conversation obj)
        {
            var chat = new Dictionary<string, object>()
            {
                { "@id", obj.Id },
                { "@created_at", DateTime.Now },
                { "@created_by", obj.CreatedBy }
            };

            bool succeeded = await ExecuteNonQuery(INSERT_STATEMENT, chat);
            bool membersAdded = false;

            if (succeeded)
            {
                var vals = new List<string>();
                var members = new Dictionary<string, object>()
                {
                    { "@conversationId", obj.Id }
                };
                for(var i = 0; i < obj.Members.Count; i++)
                {
                    vals.Add($"(@conversationId, @userId{i})");
                    members.Add($"@userId{i}", obj.Members[i]);
                }
                
                var statement = String.Format(INSERT_MEMBERS_STATEMENT, String.Join(',', vals.ToArray()));

                membersAdded = await ExecuteNonQuery(statement, members); 
            }

            return succeeded && membersAdded ? obj.Id : null;
        }

        public async Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<Conversation> Retrieve(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Conversation>> RetrieveMany(List<string> keys)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Conversation>> RetrieveUserConversations(string userId)
        {
            var conversations = new List<Conversation>();
            var users = new List<User>();
            var map = new NameValueCollection();

            using(var convoData = await ExecuteQuery(RETRIEVE_USER_CONVERSATIONS_QUERY, new Dictionary<string, object>() { { "@userId", userId } }))
            {
                conversations = convoData.ToList(x => new Conversation() { 
                    Id = x.Field<string>("id").ToString(),
                    CreatedBy = x.Field<Guid>("created_by").ToString(),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    MemberUsers = new List<User>()
                }).DistinctBy(x => x.Id).ToList();

                users = convoData.ToList(x => new User() {
                    Username = x.Field<string>("username"),
                    Email = x.Field<string>("email"),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Online = x.Field<bool>("online"),
                    ProfilePicture = x.Field<string>("picture"), 
                    Id = x.Field<Guid>("user_id").ToString() 
                }).Distinct().ToList();

                var response = new Dictionary<string, string>();
                var vals = convoData.Tables[0].AsEnumerable();

                foreach (var val in vals)
                {

                    var id = val.Field<string>("id").ToString();
                    var uId = val.Field<Guid>("user_id").ToString();

                    if (uId == userId)
                        continue;
                    
                    conversations.First(x => x.Id == id).MemberUsers.Add(users.First(x => x.Id == uId));
                }
            }

            return conversations;

        }

        public async Task<bool> Update(Conversation obj)
        {
            throw new NotImplementedException();
        }
    }
}
