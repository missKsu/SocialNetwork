using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SocialNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Users.Entities;

namespace SocialNetwork.Api
{
    public class UsersApi : BaseApi
    {
        private string address;

        public UsersApi(IConfiguration configuration)
        {
            address = configuration.GetSection("Addresses")["Users"];
        }

        public UsersApi()
        {

        }

        public User FindUser(string name)
        {
            var usersResponse = GetRequest($"{address}users/name/{name}");
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var user = JsonConvert.DeserializeObject<User>(jsonString);
            return user;
        }

        public UserModel FindUsersByName(string name)
        {
            var user = FindUser(name);
            return Convert(user);
        }

        public int FindIdUserByName(string name)
        {
            var user = FindUser(name);
            return user.Id;
        }

        public virtual UserModel FindUsersById(int id)
        {
            var usersResponse = GetRequest($"{address}users/id/{id}");
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var users = JsonConvert.DeserializeObject<User>(jsonString);
            return new UserModel { Name = users.Name};
        }

        public UserModel AddUser(UserModel userModel)
        {
            var response = PostRequest($"{address}users", Convert(userModel));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return userModel;
            return null;
        }

        private User Convert(UserModel userModel)
        {
            return new User { Name = userModel.Name };
        }

        private UserModel Convert(User user)
        {
            return new UserModel { Name = user.Name };
        }
    }
}
