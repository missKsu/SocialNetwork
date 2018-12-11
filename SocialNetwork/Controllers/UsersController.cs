﻿using System;
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
            //return RedirectToAction(nameof(GetAllUsers));
            return usersApi.AddUser(userModel);
        }

        [HttpPost("addbyif")]
        public ActionResult<UserModel> AddUserByIf(UserModel userModel)
        {
            var result = AddUser(userModel);
            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost("editbyif")]
        public ActionResult<UserModel> EditUserByIf(EditUserModel userModel)
        {
            var result = usersApi.EditUser(userModel.OriginalName, new UserModel { Name = userModel.NewName});
            return RedirectToAction(nameof(GetAllUsers));
        }

        [HttpPost("deletebyif")]
        public ActionResult<UserModel> DeleteUserByIf(DeleteUserModel userModel)
        {
            /*
            var result = usersApi.DeleteUser(userModel.Name);
            return RedirectToAction(nameof(GetAllUsers));
            */
            return null;
        }

        [HttpGet]
        public ActionResult GetAllUsers()
        {
            var model = usersApi.GetAllUsers();

            return View(model);
        }

        [HttpGet("new")]
        public ActionResult NewUser()
        {
            return View();
        }

        [HttpGet("edit")]
        public ActionResult EditUser(string name)
        {
            var user = usersApi.FindUser(name);
            var modifiedUser = new EditUserModel { OriginalName = user.Name, NewName = user.Name};
            return View(modifiedUser);
        }

        [HttpGet("delete")]
        public ActionResult DeleteUser(string name)
        {
            var user = new DeleteUserModel { Name = name};
            return View(user);
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
