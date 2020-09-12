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
    public class ConversationMessageMySqlRepository : MySQLRepository<Message>, IConversationMessageRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`conversation_message`
                                        ( 
                                        `author`,
                                        `text`,
                                        `created_at`, 
                                        `conversation_id`)
                                        VALUES
                                        ( 
                                        @author,
                                        @text,
                                        @created_at, 
                                        @conversation_id);
                                        ";

        const string RETRIEVE_FOR_CONVERSATION_QUERY = @"SELECT * 
                                                    FROM (SELECT m.*, u.email as author_email, u.online AS author_online, u.username AS author_username, u.picture AS author_picture FROM safehouse.conversation_message m
                                                    JOIN chat_user u ON u.id = m.author
                                                    WHERE conversation_id = @conversationId
                                                    ORDER BY created_at DESC
                                                    LIMIT @skip,@limit) AS messages 
                                                    ORDER BY created_at ASC;";

        public ConversationMessageMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(Message obj)
        {
            var message = new Dictionary<string, object>()
            {
                {  "@author", obj.Author.Id },
                {  "@text", obj.Text }, 
                {  "@conversation_id", obj.Channel },
                {  "@created_at", DateTime.Now }
            };

            return await ExecuteNonQueryGetId(INSERT_STATEMENT, message);
        }

        public async Task<List<Message>> RetrieveForConversation(string conversationId, int limit = 50, int page = 1)
        {
            var messages = new List<Message>();
            var queryParams = new Dictionary<string, object>() {
                { "@conversationId", conversationId },
                { "@limit", limit },
                { "@skip", (page-1)*limit}
            };
            using (var messagesData = await ExecuteQuery(RETRIEVE_FOR_CONVERSATION_QUERY, queryParams))
            {
                messages = messagesData.ToList(x => new Message()
                {
                    Channel = x.Field<string>("conversation_id").ToString(), 
                    Text = x.Field<string>("text"),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Author = new User()
                    {
                        Id = x.Field<Guid>("author").ToString(),
                        Email = x.Field<string>("author_email"),
                        Online = x.Field<bool>("author_online"),
                        Username = x.Field<string>("author_username"),
                        ProfilePicture = x.Field<string>("author_picture")
                    }
                });
            }

            return messages;
        }

        public async Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Message> Retrieve(string id)
        {
            throw new NotImplementedException();
        }
       
        public async Task<bool> Update(Message obj)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> RetrieveMany(List<string> keys)
        {
            throw new NotImplementedException();
        } 
    }
}
