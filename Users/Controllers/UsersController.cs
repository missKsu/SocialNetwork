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
        public User GetUserById(int id)
        {
            //LINQ
            return dbContext.Users.FirstOrDefault(u => u.Id == id);
        }

        [HttpGet("name/{name}")]
        public ActionResult<User> GetUserByName(string name)
        {
            return dbContext.Users.SingleOrDefault(u => u.Name == name);
        }

        [HttpPost]
        public ActionResult AddUser([FromBody]User user)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
            return StatusCode(200);
        }
    }
}
