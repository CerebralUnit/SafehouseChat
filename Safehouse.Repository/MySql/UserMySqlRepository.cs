using Safehouse.Core;
using Safehouse.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Safehouse.Repository.MySql
{
    public class UserMySqlRepository : MySQLRepository<User>, IUserRepository
    {
        const string INSERT_STATEMENT = @"INSERT INTO `safehouse`.`chat_user`
                                                (`id`,
                                                `email`,
                                                `password`,
                                                `online`,
                                                `username`,
                                                `picture`,
                                                `created_at`)
                                            VALUES
                                                (@id,
                                                @email,
                                                @password,
                                                @online,
                                                @username,
                                                @picture,
                                                @created_at);";

        const string RETRIEVE_BY_ID_QUERY = @"SELECT * FROM `safehouse`.`chat_user` WHERE id = @id";

        const string RETRIEVE_QUERY = @"SELECT * FROM `safehouse`.`chat_user` WHERE email = @email OR username = @email";

        const string RETRIEVE_MANY_QUERY = @"SELECT * FROM `safehouse`.`chat_user` WHERE {0}";

        const string SUBSCRIBE_STATEMENT = @"INSERT INTO `safehouse`.`chat_group_member`
                                                (
                                                `userId`,
                                                `chat_group_id`,
                                                `online`
                                                )
                                            VALUES
                                                (
                                                @userId,
                                                @chat_group_id,
                                                @online
                                                );";

        const string RETRIEVE_FRIENDS_QUERY = @"SELECT u.* FROM friend_request f
                                                JOIN chat_user u ON f.recipient_id = u.id
                                                WHERE sender_id = @userId AND `status` = 'Accepted'
                                                UNION
                                                SELECT u.* FROM friend_request f
                                                JOIN chat_user u ON f.sender_id = u.id
                                                WHERE recipient_id = @userId AND `status` = 'Accepted';";

        public UserMySqlRepository(string connectionString) : base(connectionString)
        {
        }

        public async Task<string> Create(User obj)
        {
            string id = Guid.NewGuid().ToString();
  
            var user = new Dictionary<string, object>()
            { 
                { "@id", id },
                { "@email", obj.Email },
                { "@password", obj.Password },
                { "@online", true },
                { "@username", obj.Username },
                { "@picture", String.Empty },
                { "@created_at", DateTime.Now }
            };
             
            if (!await ExecuteNonQuery(INSERT_STATEMENT, user))
                id = null;

            return id;
        }

        public Task<bool> Delete(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<User> RetrieveById(string id)
        {
            User user = null;

            using (var userData = await ExecuteQuery(RETRIEVE_BY_ID_QUERY, new Dictionary<string, object>() { { "@id", id } }))
            { 
                user = userData.As(x => new User()
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
            return user;
        }

        public async Task<User> Retrieve(string email)
        {
            User user = null;

            using (var userData = await ExecuteQuery(RETRIEVE_QUERY, new Dictionary<string, object>() { { "@email", email } }))
            {
                user = userData.As(x => new User()
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

            return user;
        }

        public async Task<bool> SubscribeToGroup(string groupId, string userId)
        {
            var subscription = new Dictionary<string, object>()
            {  
                { "@userId", userId },
                { "@chat_group_id", groupId }               
            };
             
            return await ExecuteNonQuery(SUBSCRIBE_STATEMENT, subscription);
        }

        public Task<bool> Update(User obj)
        {
            throw new NotImplementedException();
        }

        public async Task<List<User>> RetrieveMany(List<string> keys)
        {
            List<User> users = new List<User>();

            if (keys == null || keys.Count == 0)
                return users;
            
            var orQuery = BuildOrQuery("id", "@id", keys);

            var finalQuery = String.Format(RETRIEVE_MANY_QUERY, orQuery.WhereQuery);

           

            using (var userData = await ExecuteQuery(finalQuery, orQuery.Parameters))
            {
                 
                users = userData.ToList(x => new User()
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

        public async Task<List<User>> RetrieveFriends(string userId)
        {
            List<User> users = null;

            using(var friendData = await ExecuteQuery(RETRIEVE_FRIENDS_QUERY, new Dictionary<string, object>() { { "@userId", userId } }))
            {
                users = friendData.ToList(x => new User() {
                    Username = x.Field<string>("username"),
                    Email = x.Field<string>("email"),
                    CreatedAt = x.Field<DateTime>("created_at"),
                    Online = Convert.ToBoolean(x.Field<sbyte>("online")),
                    ProfilePicture = x.Field<string>("picture"),
                    Password = x.Field<string>("password"),
                    Id = x.Field<Guid>("id").ToString()
                });
            }

            return users;
        }
    }
}
