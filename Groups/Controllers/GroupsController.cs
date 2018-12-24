﻿using System;
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

        [HttpGet("group/{name}")]
        public ActionResult<Group> FindGroupByName(string name)
        {
            return dbContext.Groups.FirstOrDefault(g => g.Name == name);
        }

        [HttpGet("id/{id}")]
        public ActionResult<Group> FindGroupById(int id)
        {
            return dbContext.Groups.FirstOrDefault(g => g.Id == id);
        }

        [HttpPost("group")]
        public int AddGroup([FromBody]Group group)
        {
            if(group.Name != "" && group.Creator != 0)
            {
                dbContext.Groups.Add(group);
                dbContext.SaveChanges();
                return dbContext.Groups.FirstOrDefault(g => g.Creator == group.Creator && g.Name == group.Name).Id;
            }
            return -1;
        }

        [HttpGet("description/{word}")]
        public ActionResult<List<Group>> FindGroupsByDescription(string word)
        {
            var result = dbContext.Groups.Where(g => g.Description == word);
            return result.Select(s => new Group { Id = s.Id, Name = s.Name, Creator = s.Creator, Description = s.Description }).ToList();
        }

        [HttpGet("creator/{creator}")]
        public ActionResult<List<Group>> FindGroupsByCreator(int creator)
        {
            var result = dbContext.Groups.Where(g => g.Creator == creator);
            return result.Select(s => new Group { Id = s.Id, Name = s.Name, Creator = s.Creator, Description = s.Description }).ToList();
        }

        [HttpPut("group/{name}")]
        public ActionResult UpdateGroup(string name, [FromBody]Group group)
        {
            var isExist = dbContext.Groups.FirstOrDefault(g => g.Name == group.Name);
            if (isExist == null)
            {
                var group_old = dbContext.Groups.FirstOrDefault(g => g.Name == name);
                if (group_old == null)
                {
                    return StatusCode(400);
                }
                bool isChanged = false;
                if (group.Name != "")
                {
                    group_old.Name = group.Name;
                    isChanged = true;
                }
                if (group.Description != "")
                {
                    group_old.Description = group.Description;
                    isChanged = true;
                }
                if (isChanged)
                {
                    dbContext.Groups.Update(group_old);
                    dbContext.SaveChanges();
                    return StatusCode(200);
                }
                else
                    return StatusCode(422);
            }
            return StatusCode(400);
        }

        [HttpGet()]
        public ActionResult<List<Group>> GetAllGroups()
        {
            return dbContext.Groups.ToList();
        }

        [HttpDelete("group/{name}")]
        public ActionResult DeleteGroup(string name)
        {
            var group = dbContext.Groups.FirstOrDefault(g => g.Name == name);
            if (group != null)
            {
                dbContext.Groups.Remove(group);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(422);
        }
    }
}
