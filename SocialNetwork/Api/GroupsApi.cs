using Groups.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SocialNetwork.Models.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var groupResponse = GetRequest($"{address}groups/{name}");
            string jsonResponse = groupResponse.Content.ReadAsStringAsync().Result;
            var group = JsonConvert.DeserializeObject<Group>(jsonResponse);
            return group;
        }

        public Group AddGroup(Group group)
        {
            var response = PostRequest($"{address}groups", group);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return group;
            return null;
        }

        public AllGroupsModel GetAllGroups()
        {
            var groupsResponse = GetRequest($"{address}groups/");
            string jsonString = groupsResponse.Content.ReadAsStringAsync().Result;
            var groups = JsonConvert.DeserializeObject<List<Group>>(jsonString);
            return Convert(groups);
        }

        public Group Convert(GroupModel groupModel)
        {
            return new Group { Name = groupModel.Name };
        }

        public GroupModel Convert(Group group)
        {
            return new GroupModel { Name = group.Name };
        }

        private AllGroupsModel Convert(List<Group> groups)
        {
            var Gr = new AllGroupsModel { Groups = new List<GroupModel> { } };
            //var Groups = new AllGroupsModel { Groups = new List<GroupModel> { } };
            foreach (var group in groups)
            {
                Gr.Groups.Add(new GroupModel { Name = group.Name, Creator = "TestUser"});
                //Users.Users.Add(Convert(user));
            }
            return Gr;
        }
    }

 }
