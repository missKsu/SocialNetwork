using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SocialNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Users;
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
            if (!CheckAuthorization())
                return null;
            var usersResponse = GetRequest($"{address}users/name/{name}");
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (usersResponse.StatusCode != System.Net.HttpStatusCode.OK && usersResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                usersResponse = GetRequest($"{address}users/name/{name}");
                if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var user = JsonConvert.DeserializeObject<User>(jsonString);
            return user;
        }

        public UserModel FindUsersByName(string name)
        {
            var user = FindUser(name);
            if (user is null)
                return null;
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
            if (!CheckAuthorization())
                return null;
            var usersResponse = GetRequest($"{address}users/id/{id}");
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (usersResponse.StatusCode != System.Net.HttpStatusCode.OK && usersResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                usersResponse = GetRequest($"{address}users/id/{id}");
                if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            string jsonString = usersResponse.Content.ReadAsStringAsync().Result;
            var users = JsonConvert.DeserializeObject<User>(jsonString);
            if (users != null)
                return new UserModel { Name = users.Name };
            return null;
        }

        public UserModel AddUser(UserModel userModel)
        {
            if (!CheckAuthorization())
                return null;
            var response = PostRequest($"{address}users", Convert(userModel));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                response = PostRequest($"{address}users", Convert(userModel));
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return userModel;
            return null;
        }

        public UserModel AddUser(User user)
        {
            if (!CheckAuthorization())
                return null;
            var response = PostRequest($"{address}users", Convert(user));
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                response = PostRequest($"{address}users", Convert(user));
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return Convert(user);
            return null;
        }

        public UserModel EditUser(string name, string newName)
        {
            if (!CheckAuthorization())
                return null;
            var response = PutRequest($"{address}users/user/", name, newName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                response = PutRequest($"{address}users/user/", name, newName);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return new UserModel { Name = newName};
            return null;
        }

        public HttpResponseMessage DeleteUser(string name)
        {
            if (!CheckAuthorization())
                return null;
            var response = DeleteRequest($"{address}users/user/", name);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                response = DeleteRequest($"{address}users/user/", name);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            return response;
        }

        public UserModel EditUser(string name, UserModel userModel)
        {
            if (!CheckAuthorization())
                return null;
            var newName = userModel.Name;
            var response = PutRequest($"{address}users/user/",name, newName);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                response = DeleteRequest($"{address}users/user/", name);
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return userModel;
            return null;
        }

        public AllUsersModel GetAllUsers()
        {
            if (!CheckAuthorization())
                return null;
            var usersResponse = GetRequest($"{address}users/");
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            if (usersResponse.StatusCode != System.Net.HttpStatusCode.OK && usersResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (usersResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return null;
                usersResponse = GetRequest($"{address}users/");
                if (usersResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
            }
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

        private bool CheckAuthorization()
        {
            if (!Authorized)
            {
                token = Authorize($"{address}users", UsersCredenntials.Login, UsersCredenntials.Password);
                if (token is null)
                    return false;
                Authorized = true;
            }
            return true;
        }
    }
}
