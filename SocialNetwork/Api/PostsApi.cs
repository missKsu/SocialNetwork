using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Posts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialNetwork.Api
{
    public class PostsApi : BaseApi
    {
        private string address;

        public PostsApi(IConfiguration configuration)
        {
            address = configuration.GetSection("Addresses")["Posts"];
        }

        public PostsApi()
        {

        }
        
        public virtual (List<Post>,int) GetPostsByAuthor(int name, int page, int perpage)
        {
            var groupResponse = GetRequest($"{address}posts/author/{name}?page={page}&perpage={perpage}");
            string jsonResponse = groupResponse.Content.ReadAsStringAsync().Result;
            var posts = JsonConvert.DeserializeObject<(List<Post>,int)>(jsonResponse);
            return posts;
        }

        public virtual (List<Post>, int) GetPostsByGroup(int name, int page, int perpage)
        {
            var groupResponse = GetRequest($"{address}posts/group/{name}?page={page}&perpage={perpage}");
            string jsonResponse = groupResponse.Content.ReadAsStringAsync().Result;
            var posts = JsonConvert.DeserializeObject<(List<Post>, int)>(jsonResponse);
            return posts;
        }
    }
}
