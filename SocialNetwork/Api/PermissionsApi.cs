using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Permissions.Entities;
using SocialNetwork.Models.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialNetwork.Api
{
    public class PermissionsApi : BaseApi
    {
        private string address;

        public PermissionsApi(IConfiguration configuration)
        {
            address = configuration.GetSection("Addresses")["Permissions"];
        }

        public PermissionsApi()
        {

        }

        public Permission AddPermission(Permission permission)
        {
            var response = PostRequest($"{address}permissions", permission);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return permission;
            return null;
        }

        public Permission GetPermissionByFull(Permission permission)
        {
            var subjectId = permission.SubjectId;
            var objectType = permission.ObjectType;
            var objectId = permission.ObjectId;
            var response = GetRequest($"{address}permissions/{subjectId}/{objectType}/{objectId}");
            string jsonString = response.Content.ReadAsStringAsync().Result;
            if (jsonString == "[]")
                return null;
            var per = JsonConvert.DeserializeObject<Permission>(jsonString);
            return per;
        }

        public Permission GetPermissionForUserByGroup(int subjectId, int objectId)
        {
            var response = GetRequest($"{address}permissions/user/{subjectId}/group/{objectId}");
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permission = JsonConvert.DeserializeObject<Permission>(jsonString);
            return permission;
        }
        
        public List<Permission> GetUserPermissionsById(int id)
        {
            var response = GetRequest($"{address}permissions/user/{id}");
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(jsonString);
            return permissions;
        }

        public List<Permission> GetEditablePermissionsByUserId(int id)
        {
            var response = GetRequest($"{address}permissions/editable/user/{id}");
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(jsonString);
            return permissions;
        }
    }
}
