using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Identity;
using SocialNetwork.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                //userManager.AddPasswordAsync(new Identity.User { UserName = model.Name }, model.Password);
                var result = signInManager.PasswordSignInAsync(model.Name, model.Password, true , false).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("GetAllUsersByIf", "Users"); //View();
                    return StatusCode(200);
                }
            }
            return RedirectToAction("MessagePage","Gateway", new { message = "No such user!" });
            return StatusCode(403);
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User { UserName = model.Name, Id = Guid.NewGuid().ToString() };
                var result = userManager.CreateAsync(user, model.Password).Result;
                if (result.Succeeded)
                {
                    return Login(model);
                }
            }
            return StatusCode(403);
        }
    }
}
