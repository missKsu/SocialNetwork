using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SocialNetwork.Api;
using SocialNetwork.Models.Groups;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    [Route("groups")]
    public class GroupsController : Controller
    {
        private readonly GroupsApi groupsApi;
        private readonly ILogger<GroupsController> logger;

        public GroupsController(GroupsApi groupsApi, ILogger<GroupsController> logger)
        {
            this.groupsApi = groupsApi;
            this.logger = logger;
        }

        ////[HttpGet("{name}")]
        //public ActionResult<Group> FindGroupByName(string name)
        //{
        //    logger.LogCritical("In find group by name");
        //    return groupsApi.FindGroupByName(name);
        //}
    }
}
