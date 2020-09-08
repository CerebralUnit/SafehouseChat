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
    public class MessageMySqlRepository : MySQLRepository<Message>, IMessageRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`chat_message`
                                        ( 
                                        `author`,
                                        `text`,
                                        `created_at`,
                                        `chat_group_id`,
                                        `chat_group_channel_id`)
                                        VALUES
                                        ( 
                                        @author,
                                        @text,
                                        @created_at,
                                        @chat_group_id,
                                        @chat_group_channel_id);
                                        ";

        const string RETRIEVE_FOR_CHANNEL_QUERY = @"SELECT * 
                                                    FROM (SELECT m.*, u.email as author_email, u.online AS author_online, u.username AS author_username, u.picture AS author_picture FROM safehouse.chat_message m
                                                    JOIN chat_user u ON u.id = m.author
                                                    WHERE chat_group_channel_id = @chatGroupChannelId
                                                    ORDER BY created_at DESC
                                                    LIMIT @skip,@limit) AS messages 
                                                    ORDER BY created_at ASC;";

        public MessageMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(Message obj)
        {
            var message = new Dictionary<string, object>()
            {
                {  "@author", obj.Author.Id },
                {  "@text", obj.Text },
                {  "@chat_group_id", obj.ChatGroup },
                {  "@chat_group_channel_id", obj.Channel },
                {  "@created_at", DateTime.Now },
            };

            return await ExecuteNonQueryGetId(INSERT_STATEMENT, message);
        }

        public async Task<List<Message>> RetrieveForChannel(string channelId, int limit = 50, int page = 1)
        {
            var messages = new List<Message>();
            var queryParams = new Dictionary<string, object>() {
                { "@chatGroupChannelId", channelId },
                { "@limit", limit },
                { "@skip", (page-1)*limit}
            };
            using (var messagesData = await ExecuteQuery(RETRIEVE_FOR_CHANNEL_QUERY, queryParams))
            {
                messages = messagesData.ToList(x => new Message()
                {
                    Channel = x.Field<int>("chat_group_channel_id").ToString(),
                    ChatGroup = x.Field<int>("chat_group_id").ToString(),
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
