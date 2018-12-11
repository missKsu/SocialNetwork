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

        public UsersController(UsersDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("id/{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            //LINQ
            return dbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        [HttpGet("name/{name}")]
        public ActionResult<User> GetUserByName(string name)
        {
            return dbContext.Users.SingleOrDefault(u => u.Name == name);
        }

        [HttpGet()]
        public ActionResult<List<User>> GetAllUsers()
        {
            return dbContext.Users.ToList();
        }

        [HttpPost]
        public ActionResult AddUser([FromBody]User user)
        {
            if (user.Name != "" && user.Name != null)
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
            var user = dbContext.Users.FirstOrDefault(u => u.Name == name);
            if(user != null)
            {
                dbContext.Users.Remove(user);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(422);
        }

        [HttpGet()]
        public ActionResult<List<User>> GetAllUsers()
        {
            return dbContext.Users.ToList();
        }
    }
}
