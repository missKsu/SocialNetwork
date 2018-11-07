using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Groups.Controllers
{
    [Route("groups")]
    public class GroupsController : Controller
    {
        private readonly GroupsDbContext dbContext;

        public GroupsController(GroupsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("{name}")]
        public ActionResult<Group> FindGroupByName(string name)
        {
            return dbContext.Groups.FirstOrDefault(g => g.Name == name);
        }

        [HttpPost]
        public ActionResult AddGroup([FromBody]Group group)
        {
            dbContext.Groups.Add(group);
            dbContext.SaveChanges();
            return StatusCode(200);
        }

    }
}
