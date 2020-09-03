using MongoDB.Bson;
using MongoDB.Driver;
using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Repository
{
    public class MessageMongoRepository : IMessageRepository
    {
        public async Task<string> Create(Message obj)
        { 
            var client = new MongoClient("mongodb://localhost");
            
            var database = client.GetDatabase("minicord");
            
            var collection = database.GetCollection<BsonDocument>("messages");

            var doc = new BsonDocument();

            doc.Add("author", new BsonObjectId(obj.Author.Id));
            doc.Add("text", obj.Text);
            doc.Add("created_at", DateTime.Now);
           
            await collection.InsertOneAsync(doc);

            return doc.GetValue("_id").AsObjectId.ToString();
        }

        public async Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
        
        public async Task<Message> Retrieve(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(id));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("messages");

            var results = await collection.Aggregate().Match(filter).Lookup("user", "author", "_id", "author").FirstAsync();

            var author = results.GetValue("author").AsBsonDocument;

            return new Message()
            {
                Author = new User()
                {
                    Username = author.GetValue("username").AsString,
                    Email = author.GetValue("email").AsString,
                    CreatedAt = author.GetValue("created_at").AsDateTime,
                    Friends = author.GetValue("friends").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                    Online = author.GetValue("online").AsBoolean,
                    ProfilePicture = author.GetValue("profile_picture").AsString,
                    Password = author.GetValue("password").AsString,
                    Channels = author.GetValue("channels").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                    Id = author.GetValue("_id").AsObjectId.ToString(),
                },
                Text = results.GetValue("email").AsString,
                CreatedAt = results.GetValue("created_at").AsDateTime
            };
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
