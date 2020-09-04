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

        const string RETRIEVE_FOR_CHANNEL_QUERY = @"SELECT * WHERE chat_group_channel_id = @channelId";

        public MessageMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(Message obj)
        {
            var message = new Dictionary<string, object>()
            {
                {  "@author", obj.Author },
                {  "@text", obj.Text },
                {  "@chat_group_id", obj.ChatGroup },
                {  "@chat_group_channel_id", obj.Channel },
                {  "@created_at", DateTime.Now },
            };

            return await ExecuteNonQueryGetId(INSERT_STATEMENT, message);
        }

        public async Task<List<Message>> RetrieveForChannel(string channelId)
        {
            var messages = new List<Message>();

            using (var messagesData = await ExecuteQuery(RETRIEVE_FOR_CHANNEL_QUERY, new Dictionary<string, object>() { { "@channelId", channelId } }))
            {
                messages = messagesData.ToList(x => new Message()
                {
                    Channel = x.Field<string>("chat_group_channel_id"),
                    ChatGroup = x.Field<string>("chat_group_id"),
                    Text = x.Field<string>("text"),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Author = new User()
                    {
                        Id = x.Field<string>("author")
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
