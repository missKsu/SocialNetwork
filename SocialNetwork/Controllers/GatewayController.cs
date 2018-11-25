using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api;
using SocialNetwork.Models.Groups;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    public class GatewayController : Controller
    {
        private readonly UsersApi usersApi;
        private readonly GroupsApi groupsApi;

        public GatewayController(UsersApi usersApi, GroupsApi groupsApi)
        {
            this.usersApi = usersApi;
            this.groupsApi = groupsApi;
        }

        [HttpGet("groups/{name}")]
        public ActionResult<GroupModel> FindGroupByName(string name)
        {
            var group = groupsApi.FindGroupByName(name);
            var userName = usersApi.FindUsersById(group.Creator);
            return new GroupModel { Name = group.Name, Creator = userName.Name, Description = group.Description };
        }

        [HttpPost("groups")]
        public ActionResult<GroupModel> AddGroup(GroupModel group)
        {
            var creator = usersApi.FindIdUserByName(group.Creator);
            if(creator == 0)
            {
                return null;
            }
            var body = groupsApi.Convert(group);
            body.Creator = creator;
            var response = groupsApi.AddGroup(body);
            return group;
        }
        
        [HttpPut("groups/merge/{group}")]
        public ActionResult<GroupModel> MergeOneGroupWithAnother(string group, string with)
        {

            return null;
        }
    }
}
