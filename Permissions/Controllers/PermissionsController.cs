using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Permissions.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Permissions.Controllers
{
    [Route("permissions")]
    public class PermissionsController : Controller
    {
        private readonly PermissionsDbContext dbContext;

        public PermissionsController(PermissionsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("user/{id}")]
        public ActionResult<List<Permission>> GetUserPermissionsById(int id)
        {
            var result = dbContext.Permissions.Where(p => p.SubjectId == id);
            return result.Select(s => new Permission { Id = s.Id, SubjectType = s.SubjectType, SubjectId = s.SubjectId, ObjectType = s.ObjectType, ObjectId = s.ObjectId, Operation = s.Operation }).ToList();
        }

        [HttpGet("{objectType}/{id}/{operation}")]
        public ActionResult<List<Permission>> GetUserPermissionsByObjectAndOperation(Entities.Object objectType ,int id, Operation operation)
        {
            var result = dbContext.Permissions.Where(p => p.ObjectType == objectType && p.ObjectId == id && p.Operation == operation);
            return result.Select(s => new Permission { Id = s.Id, SubjectType = s.SubjectType, SubjectId = s.SubjectId, ObjectType = s.ObjectType, ObjectId = s.ObjectId, Operation = s.Operation }).ToList();
        }

        [HttpGet("user/{subjectId}/group/{objectId}")]
        public ActionResult<Permission> GetUserPermissionsForUserByGroup(int subjectId, int objectId)
        {
            var result = dbContext.Permissions.FirstOrDefault(p => p.ObjectType == Entities.Object.Group && p.ObjectId == objectId && p.SubjectId == subjectId && p.SubjectType == Subject.User);
            return result;
        }

        [HttpPost]
        public ActionResult AddPermission([FromBody]Permission permission)
        {
            if (permission.ObjectId != 0 && permission.SubjectId != 0)
            {
                dbContext.Permissions.Add(permission);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }
    }
}
