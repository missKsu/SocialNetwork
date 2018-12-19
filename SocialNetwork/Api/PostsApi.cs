using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Posts.Entities;
using SocialNetwork.Models.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public Post GetPostById(int id)
        {
            var postResponse = GetRequest($"{address}posts/id/{id}");
            string jsonResponse = postResponse.Content.ReadAsStringAsync().Result;
            var post = JsonConvert.DeserializeObject<Post>(jsonResponse);
            return post;
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

        public Post AddPost(Post post)
        {
            var response = PostRequest($"{address}posts/post", post);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return post;
            return null;
        }

        public HttpResponseMessage EditPost(int id, string newText)
        {
            var response = PutRequest($"{address}posts/post/", id, new Post { Text = newText });
            return response;
        }

        public HttpResponseMessage DeletePost(int id)
        {
            var postResponse = DeleteRequest($"{address}posts/post/", id);
            return postResponse;
        }

        public Post Convert(PostModel postModel)
        {
            return new Post { Id = postModel.Id, Text = postModel.Text };
        }
    }
}
