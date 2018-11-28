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
            if (response.
                == System.Net.HttpStatusCode.OK)
                return group;
            return null;
        }

        public Group Convert(GroupModel groupModel)
        {
            return new Group { Name = groupModel.Name };
        }

        public GroupModel Convert(Group group)
        {
            return new GroupModel { Name = group.Name };
        }
    }

 }
