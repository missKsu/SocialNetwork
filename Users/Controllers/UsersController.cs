using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Users.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Users.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly UsersDbContext dbContext;
        private readonly TokensStorage tokensStorage;

        public UsersController(UsersDbContext dbContext, TokensStorage tokensStorage)
        {
            this.dbContext = dbContext;
            this.tokensStorage = tokensStorage;
        }

        [HttpPost("auth")]
        public string Auth([FromBody]Auth auth)
        {
            if (auth.Login == UsersCredenntials.Login && auth.Pass == UsersCredenntials.Password)
            {
                var token = Guid.NewGuid().ToString();
                tokensStorage.AddToken(token);
                return token;
            }
            return null;
        }

        public ActionResult<bool> CheckToken(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return StatusCode(400);
            }
            var token = httpContext.Request.Headers["Authorization"].ToString();
            token = token.Substring(token.IndexOf(' ') + 1);
            var check = tokensStorage.CheckToken(token);
            if (check == AuthorizeResult.NoToken || check == AuthorizeResult.WrongToken)
            {
                return StatusCode(403);
            }
            if (check == AuthorizeResult.TokenExpired)
                return StatusCode(401);
            return true;
        }

        [HttpGet("id/{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            return dbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        [HttpGet("name/{name}")]
        public ActionResult<User> GetUserByName(string name)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            return dbContext.Users.SingleOrDefault(u => u.Name.ToUpperInvariant() == name.ToUpperInvariant());
        }

        [HttpGet()]
        public ActionResult<List<User>> GetAllUsers()
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            return dbContext.Users.ToList();
        }

        [HttpPost]
        public ActionResult AddUser([FromBody]User user)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            if (user.Name != "" && user.Name != null && user.Password != "")
            {
                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        [HttpPut("user/{name}")]
        public ActionResult UpdateUser(string name, [FromBody]string newName)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var isExist = dbContext.Users.FirstOrDefault(u => u.Name == newName);
            if (isExist == null)
            {
                var user = dbContext.Users.FirstOrDefault(u => u.Name == name);
                if (user == null)
                {
                    return StatusCode(400);
                }
                user.Name = newName;
                dbContext.Users.Update(user);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(400);
        }

        [HttpDelete("user/{name}")]
        public ActionResult DeleteUser(string name)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var user = dbContext.Users.FirstOrDefault(u => u.Name == name);
            if(user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(422);
        }
    }
}
