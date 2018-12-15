using Groups.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SocialNetwork.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialNetwork.Api
{
    public class GroupsApi : BaseApi
    {
        private string address;

        public GroupsApi(IConfiguration configuration)
        {
            address = configuration.GetSection("Addresses")["Groups"];
        }

        public GroupsApi()
        {

        }

        public virtual Group FindGroupByName(string name)
        {
            var groupResponse = GetRequest($"{address}groups/group/{name}");
            string jsonResponse = groupResponse.Content.ReadAsStringAsync().Result;
            var group = JsonConvert.DeserializeObject<Group>(jsonResponse);
            return group;
        }

        public virtual Group FindGroupById(int id)
        {
            var groupResponse = GetRequest($"{address}groups/id/{id}");
            string jsonResponse = groupResponse.Content.ReadAsStringAsync().Result;
            var group = JsonConvert.DeserializeObject<Group>(jsonResponse);
            return group;
        }

        public Group AddGroup(Group group)
        {
            var response = PostRequest($"{address}groups/group", group);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return group;
            return null;
        }

        public HttpResponseMessage EditGroup(string name, string newName, string newDescription)
        {
            var response = PutRequest($"{address}groups/group/",name, new Group { Name = newName, Description = newDescription});
            return response;
        }

        public List<Group> GetAllGroups()
        {
            var groupsResponse = GetRequest($"{address}groups/");
            string jsonString = groupsResponse.Content.ReadAsStringAsync().Result;
            var groups = JsonConvert.DeserializeObject<List<Group>>(jsonString);
            return groups;
        }

        public HttpResponseMessage DeleteGroup(string name)
        {
            var groupResponse = DeleteRequest($"{address}groups/group/", name);
            return groupResponse;
        }

        public Group Convert(GroupModel groupModel)
        {
            return new Group { Name = groupModel.Name, Description = groupModel.Description };
        }

        public GroupModel Convert(Group group)
        {
            return new GroupModel { Name = group.Name, Description = group.Description };
        }
    }

 }
