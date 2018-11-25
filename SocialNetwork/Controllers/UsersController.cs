﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpPost()]
        public ActionResult<UserModel> AddUser(UserModel userModel)
        {
            return RedirectToAction(nameof(GetAllUsers));
            //return usersApi.AddUser(userModel);
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var users = new[]
            {
                new UserModel{ Name = "Ivan"},
                new UserModel{ Name = "John"}
            };

            var model = new AllUsersModel { Users = users.ToList() };

            return View(model);
        }

        [HttpGet("new")]
        public ActionResult NewUser()
        {
            return View();
        }
    }
}
