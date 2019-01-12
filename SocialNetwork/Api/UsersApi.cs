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
            if (user == null)
                return -1;
            return user.Id;
        }

        public virtual UserModel FindUsersById(int id)
        {
            var usersResponse = GetRequest($"{address}users/id/{id}");
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var users = JsonConvert.DeserializeObject<User>(jsonString);
            if (users != null)
                return new UserModel { Name = users.Name };
            return null;
        }

        public UserModel AddUser(UserModel userModel)
        {
            var response = PostRequest($"{address}users", Convert(userModel));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return userModel;
            return null;
        }

        public UserModel AddUser(User user)
        {
            var response = PostRequest($"{address}users", Convert(user));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Convert(user);
            return null;
        }

        public UserModel EditUser(string name, string newName)
        {
            var response = PutRequest($"{address}users/user/", name, newName);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return new UserModel { Name = newName};
            return null;
        }

        public HttpResponseMessage DeleteUser(string name)
        {
            var response = DeleteRequest($"{address}users/user/", name);
            return response;
        }

        public UserModel EditUser(string name, UserModel userModel)
        {
            var newName = userModel.Name;
            var response = PutRequest($"{address}users/user/",name, newName);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return userModel;
            return null;
        }

        public AllUsersModel GetAllUsers()
        {
            var usersResponse = GetRequest($"{address}users/");
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var users = JsonConvert.DeserializeObject<List<User>>(jsonString);
            return Convert(users);
        }

        private User Convert(UserModel userModel)
        {
            return new User { Name = userModel.Name, Password = userModel.Password };
        }

        private UserModel Convert(User user)
        {
            return new UserModel { Name = user.Name, Password = user.Password };
        }

        private AllUsersModel Convert(List<User> users)
        {
            var Users = new AllUsersModel { Users = new List<UserModel> { } };
            foreach (var user in users)
            {
                Users.Users.Add(Convert(user));
            }
            return Users;
        }
    }
}
