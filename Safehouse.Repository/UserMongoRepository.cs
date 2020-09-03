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
    public class UserMongoRepository : IUserRepository
    {
        public async Task<string> Create(User obj)
        { 
            var client = new MongoClient("mongodb://localhost");
            
            var database = client.GetDatabase("minicord");
            
            var collection = database.GetCollection<BsonDocument>("users");

            var doc = new BsonDocument();

            doc.Add("email", obj.Username);
            doc.Add("password", obj.Password);
            doc.Add("channels", BsonValue.Create(new List<string>()));
            doc.Add("profile_picture", String.Empty);
            doc.Add("friends", BsonValue.Create(new List<string>()));
            doc.Add("online", true);  
            doc.Add("created_at", DateTime.Now); 
            doc.Add("username", obj.Username);
         
            await collection.InsertOneAsync(doc);

            return doc.GetValue("_id").AsObjectId.ToString();
        }

        public Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
        public async Task<User> RetrieveById(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(id));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("users");

            var results = await collection.Find(filter).FirstAsync();

            return new User()
            {
                Username = results.GetValue("username").AsString,
                Email = results.GetValue("email").AsString,
                CreatedAt = results.GetValue("created_at").AsDateTime,
                Friends = results.GetValue("friends").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                Online = results.GetValue("online").AsBoolean,
                ProfilePicture = results.GetValue("profile_picture").AsString,
                Password = results.GetValue("password").AsString,
                Channels = results.GetValue("channels").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                Id = results.GetValue("_id").AsObjectId.ToString(),
            };
        }
        public async Task<User> Retrieve(string email)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("email", email);
            
            var client = new MongoClient("mongodb://localhost");
            
            var database = client.GetDatabase("minicord");
            
            var collection = database.GetCollection<BsonDocument>("users");

            var results = await collection.Find(filter).FirstAsync();

            return new User()
            {
                Username = results.GetValue("username").AsString,
                Email = results.GetValue("email").AsString,
                CreatedAt = results.GetValue("created_at").AsDateTime,
                Friends = results.GetValue("friends").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                Online = results.GetValue("online").AsBoolean,
                ProfilePicture = results.GetValue("profile_picture").AsString,
                Password = results.GetValue("password").AsString,
                Channels = results.GetValue("channels").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                Id = results.GetValue("_id").AsObjectId.ToString(),
            };
        }
        public async Task<bool> SubscribeToChannel(string channelId, string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(userId));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");

            var update = Builders<BsonDocument>.Update.Push("channels", new BsonObjectId(channelId));

            var results = await collection.UpdateOneAsync(
                filter,
                update
            );

            return results.IsAcknowledged;
        }
        public Task<bool> Update(User obj)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> RetrieveMany(List<string> keys)
        {
            throw new NotImplementedException();
        }
    }
}
