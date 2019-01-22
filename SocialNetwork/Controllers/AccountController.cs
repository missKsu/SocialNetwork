using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SocialNetwork.Identity;
using SocialNetwork.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialNetwork.Controllers
{
    [Route("account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly TokensStorage tokensStorage;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, TokensStorage tokensStorage)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.tokensStorage = tokensStorage;
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

        [HttpGet("oauth/login")]
        public IActionResult OAuthLogin()
        {
            string token = null;
            using (var client = new HttpClient())
            {
                var res = client.GetAsync("http://localhost:5005/connect/authorize?client_id=test-client&redirect_uri=http://localhost:5000/account/oauth/code&response_type=code&scope=offline_access").Result;
                token = res.Content.ReadAsStringAsync().Result;
            }
            HttpContext.Response.Headers.Add("Set-Cookie", $"api-token={token}");
            
            return base.StatusCode(200, token);
        }

        [HttpGet("oauth/relogin")]
        public IActionResult OAuthReLogin(string token)
        {
            var refresh_token = tokensStorage.RefreshToken(token);
            using (var client = new HttpClient())
            {
                var res = client.PostAsync("http://localhost:5005/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", "test-client" },
                    { "client_secret", "test-secret" },
                    { "grant_type", "authorization_code" },
                    { "scope", "offline_access" },
                    { "redirect_uri", "http://localhost:5000/account/oauth/code" },
                    { "refresh_token", refresh_token }
                })).Result;
                var str = res.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<OAuthResponse>(str);
                token = obj.AccessToken;
                tokensStorage.AddToken(token, obj.RefreshToken);
                return base.StatusCode(200, token);
            }
        }

        [HttpGet("oauth/showcode")]
        public IActionResult OAuthShowCodeCallback(string code, string scope)
        {
            using (var client = new HttpClient())
            {
                return base.StatusCode(200, code);
            }
        }

        [HttpGet("oauth/code")]
        public IActionResult OAuthCodeCallback(string code, string scope)
        {
            using (var client = new HttpClient())
            {
                var res = client.PostAsync("http://localhost:5005/connect/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", "test-client" },
                    { "client_secret", "test-secret" },
                    { "grant_type", "authorization_code" },
                    { "scope", scope },
                    { "redirect_uri", "http://localhost:5000/account/oauth/code" },
                    { "code", code }
                })).Result;
                var str = res.Content.ReadAsStringAsync().Result;
                var obj = JsonConvert.DeserializeObject<OAuthResponse>(str);
                var token = obj.AccessToken;
                tokensStorage.AddToken(token, obj.RefreshToken);
                return base.StatusCode(200, token);
            }
        }
        private class OAuthResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken;

            [JsonProperty("expires_in")]
            public int ExpiresIn;

            [JsonProperty("refresh_token")]
            public string RefreshToken;
        }
    }
}
