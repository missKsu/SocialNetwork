using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api;
using SocialNetwork.Models.Users;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly UsersApi usersApi;

        public UsersController(UsersApi usersApi)
        {
            this.usersApi = usersApi;
        }

        [HttpGet("name/{name}")]
        public ActionResult<UserModel> FindUsersByModel(string name)
        {
            return usersApi.FindUsersByName(name);
        }

        [HttpPost]
        public ActionResult<UserModel> AddUser(UserModel userModel)
        {
            return usersApi.AddUser(userModel);
        }

        [HttpPut("user/{name}")]
        public ActionResult<UserModel> EditUser(string name, string newName)
        {
            return usersApi.EditUser(name,newName);
        }

        [HttpGet]
        public ActionResult<AllUsersModel> GetAllUsers()
        {
            return usersApi.GetAllUsers();
        }

        [HttpDelete("user/{name}")]
        public ActionResult<HttpStatusCode> DeleteUser(string name)
        {
            var result = usersApi.DeleteUser(name);
            return result.StatusCode;
        }
    }
}
