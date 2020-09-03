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
    public class ChannelMongoRepository : IChannelRepository
    {
        public async Task<string> Create(Channel obj)
        { 
            var client = new MongoClient("mongodb://localhost");
            
            var database = client.GetDatabase("minicord");
            
            var collection = database.GetCollection<BsonDocument>("channels");

            var doc = new BsonDocument();

            doc.Add("message", BsonValue.Create(obj.MessageIds));
            doc.Add("participant", BsonValue.Create(obj.ParticipantIds)); 
            doc.Add("creator", obj.Creator);
            doc.Add("channel_name", obj.Name);
            doc.Add("channel_picture", obj.Picture);  
            doc.Add("created_at", DateTime.Now);  
         
            await collection.InsertOneAsync(doc);
             
            return doc.GetValue("_id").AsObjectId.ToString();
        }

        public Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }
       
        public async Task<bool> AddParticipant(string channelId, string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(channelId));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");

            var update = Builders<BsonDocument>.Update.Push("participant", new BsonObjectId(userId));

            var results = await collection.UpdateOneAsync(
                filter,
                update
            );

            return results.IsAcknowledged;
        }


        public async Task<bool> RemoveParticipant(string channelId, string userId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(channelId));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");

            var update = Builders<BsonDocument>.Update.Pull("participant", new BsonObjectId(userId));

            var results = await collection.UpdateOneAsync(
                filter,
                update
            );

            return results.IsAcknowledged;
        }


        public async Task<Channel> Retrieve(string id)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonObjectId(id));

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");


            var channels = database.GetCollection<BsonDocument>("channels");
            var users = database.GetCollection<BsonDocument>("users");

            var resultsList = channels
                                .Aggregate()
                                .Match(filter)
                                .Lookup("users", "participant", "_id", "participants")
                                .Lookup("messages", "message", "_id", "messages") 
                                .Lookup("users", "messages.author", "_id", "messageAuthors")  
                                .ToList();

            var results = resultsList.First();
            var messageArray = results.GetValue("messages").AsBsonArray;
            var authorArray = results.GetValue("messageAuthors").AsBsonArray;
            return new Channel()
            {
                Messages = results.GetValue("messages").AsBsonArray.Select((x) =>
                {
                    var messageDoc = x.AsBsonDocument;
                    var index = messageArray.IndexOf(x);
                    var authorUserDoc = authorArray[index].AsBsonDocument;
                    
                    return new Message()
                    {
                        Author = new User()
                        {
                            Username = authorUserDoc.GetValue("username").AsString,
                            Email = authorUserDoc.GetValue("email").AsString,
                            CreatedAt = authorUserDoc.GetValue("created_at").AsDateTime,
                            Friends = authorUserDoc.GetValue("friends").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                            Online = authorUserDoc.GetValue("online").AsBoolean,
                            ProfilePicture = authorUserDoc.GetValue("profile_picture").AsString,
                            Password = authorUserDoc.GetValue("password").AsString,
                            Channels = authorUserDoc.GetValue("channels").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                            Id = authorUserDoc.GetValue("_id").AsObjectId.ToString(),
                        },
                        CreatedAt = messageDoc.GetValue("created_at").AsDateTime,
                        Text = messageDoc.GetValue("text").AsString
                    };
                }).ToList() ,
                Participants = results.GetValue("participants").AsBsonArray.Select((x) =>
                {
                    var participantDoc = x.AsBsonDocument;
                    return new User()
                    {
                        Username = participantDoc.GetValue("username").AsString,
                        Email = participantDoc.GetValue("email").AsString,
                        CreatedAt = participantDoc.GetValue("created_at").AsDateTime,
                        Friends = participantDoc.GetValue("friends").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                        Online = participantDoc.GetValue("online").AsBoolean,
                        ProfilePicture = participantDoc.GetValue("profile_picture").AsString,
                        Password = participantDoc.GetValue("password").AsString,
                        Channels = participantDoc.GetValue("channels").AsBsonArray.Values.Select(x => x.AsObjectId.ToString()).ToList(),
                        Id = participantDoc.GetValue("_id").AsObjectId.ToString(),
                    };
                }
                ).ToList(),
                CreatedAt = results.GetValue("created_at").AsDateTime,
                Creator = results.GetValue("creator").AsString,
                Name = results.GetValue("channel_name").AsString,
                Picture = results.GetValue("channel_picture").AsString, 
                Id = results.GetValue("_id").AsObjectId.ToString()
            };
        }

        public async Task<List<Channel>> RetrieveMany(List<string> keys)
        {
            var values = new List<BsonValue>();

            foreach(var key in keys)
            {
                values.Add(BsonObjectId.Create(key));
            }

            var filter = Builders<BsonDocument>.Filter.In("_id", values);

            var client = new MongoClient("mongodb://localhost");

            var database = client.GetDatabase("minicord");

            var collection = database.GetCollection<BsonDocument>("channels");
            var channels = new List<Channel>();

            await collection.Find(filter).ForEachAsync((x) =>
                channels.Add(new Channel()
                {
                    MessageIds = x.GetValue("message").IsBsonNull ? new List<string>() : x.GetValue("message").AsBsonArray.Select(x => x.IsObjectId ? x.AsObjectId.ToString() : x.AsString).ToList(),
                    ParticipantIds = x.GetValue("participant").IsBsonNull ? new List<string>() : x.GetValue("participant").AsBsonArray.Select(x => x.IsObjectId ? x.AsObjectId.ToString() : x.AsString).ToList(),
                    CreatedAt = x.GetValue("created_at").AsDateTime,
                    Creator = x.GetValue("creator").AsString,
                    Name = x.GetValue("channel_name").AsString,
                    Picture = x.GetValue("channel_picture").AsString,
                    Id = x.GetValue("_id").AsObjectId.ToString(),
                })
            );

            return channels;
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
