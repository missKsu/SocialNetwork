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
        private readonly TokensStorage tokensStorage;

        public PermissionsController(PermissionsDbContext dbContext, TokensStorage tokensStorage)
        {
            this.dbContext = dbContext;
            this.tokensStorage = tokensStorage;
        }

        [HttpPost("auth")]
        public string Auth([FromBody]Auth auth)
        {
            if (auth.Login == PermissionsCredentials.Login && auth.Pass == PermissionsCredentials.Password)
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


        [HttpGet("user/{id}")]
        public ActionResult<List<Permission>> GetUserPermissionsById(int id)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var result = dbContext.Permissions.Where(p => p.SubjectId == id);
            return result.Select(s => new Permission { Id = s.Id, SubjectType = s.SubjectType, SubjectId = s.SubjectId, ObjectType = s.ObjectType, ObjectId = s.ObjectId, Operation = s.Operation }).ToList();
        }

        [HttpGet("editable/user/{id}")]
        public ActionResult<List<Permission>> GetEditablePermissionsByUserId(int id)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var response = dbContext.Permissions.Where(p => p.SubjectId == id && p.Operation == Operation.Admin);
            var result = response.Select(s => new Permission { ObjectType = s.ObjectType, ObjectId = s.ObjectId}).ToList();
            var editablePermissions = new List<Permission> { };
            foreach (var permission in result)
            {
                var responsePermission = dbContext.Permissions.Where(p => p.ObjectType == permission.ObjectType && p.ObjectId == permission.ObjectId).ToList();
                foreach (var resPermission in responsePermission)
                {
                    editablePermissions.Add(resPermission);
                }
            }
            return editablePermissions;
        }

        [HttpGet("{objectType}/{id}/{operation}")]
        public ActionResult<List<Permission>> GetUserPermissionsByObjectAndOperation(Entities.Object objectType ,int id, Operation operation)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var result = dbContext.Permissions.Where(p => p.ObjectType == objectType && p.ObjectId == id && p.Operation == operation);
            return result.Select(s => new Permission { Id = s.Id, SubjectType = s.SubjectType, SubjectId = s.SubjectId, ObjectType = s.ObjectType, ObjectId = s.ObjectId, Operation = s.Operation }).ToList();
        }

        [HttpGet("/{subjectId}/{objectType}/{id}")]
        public ActionResult<Permission> GetPermissionsByFull(Entities.Object objectType,int subjectId, int id, Entities.Object objectype)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var result = dbContext.Permissions.FirstOrDefault(p => p.ObjectType == objectType && p.ObjectId == id && p.SubjectId == subjectId);
            return result;
        }

        [HttpGet("user/{subjectId}/group/{objectId}")]
        public ActionResult<Permission> GetUserPermissionsForUserByGroup(int subjectId, int objectId)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var result = dbContext.Permissions.FirstOrDefault(p => p.ObjectType == Entities.Object.Group && p.ObjectId == objectId && p.SubjectId == subjectId && p.SubjectType == Subject.User);
            return result;
        }

        [HttpPost]
        public ActionResult AddPermission([FromBody]Permission permission)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
