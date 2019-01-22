using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Permissions;
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
            CheckAuthorization();
            var response = PostRequest($"{address}permissions", permission);
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                response = PostRequest($"{address}permissions", permission);
            }
            return permission;
        }

        public Permission GetPermissionByFull(Permission permission)
        {
            CheckAuthorization();
            var subjectId = permission.SubjectId;
            var objectType = permission.ObjectType;
            var objectId = permission.ObjectId;
            var response = GetRequest($"{address}permissions/{subjectId}/{objectType}/{objectId}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                response = GetRequest($"{address}permissions/{subjectId}/{objectType}/{objectId}");
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            if (jsonString == "[]")
                return null;
            var per = JsonConvert.DeserializeObject<Permission>(jsonString);
            return per;
        }

        public Permission GetPermissionForUserByGroup(int subjectId, int objectId)
        {
            CheckAuthorization();
            var response = GetRequest($"{address}permissions/user/{subjectId}/group/{objectId}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                response = GetRequest($"{address}permissions/user/{subjectId}/group/{objectId}");
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permission = JsonConvert.DeserializeObject<Permission>(jsonString);
            return permission;
        }
        
        public List<Permission> GetUserPermissionsById(int id)
        {
            CheckAuthorization();
            var response = GetRequest($"{address}permissions/user/{id}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                response = GetRequest($"{address}permissions/user/{id}");
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(jsonString);
            return permissions;
        }

        public List<Permission> GetEditablePermissionsByUserId(int id)
        {
            CheckAuthorization();
            var response = GetRequest($"{address}permissions/editable/user/{id}");
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                response = GetRequest($"{address}permissions/editable/user/{id}");
            }
            string jsonString = response.Content.ReadAsStringAsync().Result;
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(jsonString);
            return permissions;
        }

        private void CheckAuthorization()
        {
            if (!Authorized)
            {
                token = Authorize($"{address}permissions", PermissionsCredentials.Login, PermissionsCredentials.Password);
                Authorized = true;
            }
        }
    }
}
