using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Posts;
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
            CheckAuthorization();
            var postResponse = GetRequest($"{address}posts/id/{id}");
            if (postResponse.StatusCode != System.Net.HttpStatusCode.OK && postResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (postResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                postResponse = GetRequest($"{address}posts/id/{id}");
            }
            string jsonResponse = postResponse.Content.ReadAsStringAsync().Result;
            var post = JsonConvert.DeserializeObject<Post>(jsonResponse);
            return post;
        }
        
        public virtual (List<Post>,int) GetPostsByAuthor(int name, int page, int perpage)
        {
            CheckAuthorization();
            var postResponse = GetRequest($"{address}posts/author/{name}?page={page}&perpage={perpage}");
            if (postResponse.StatusCode != System.Net.HttpStatusCode.OK && postResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return (null,0);
            if (postResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                postResponse = GetRequest($"{address}posts/author/{name}?page={page}&perpage={perpage}");
            }
            string jsonResponse = postResponse.Content.ReadAsStringAsync().Result;
            var posts = JsonConvert.DeserializeObject<(List<Post>,int)>(jsonResponse);
            return posts;
        }

        public virtual (List<Post>, int) GetPostsByGroup(int name, int page, int perpage)
        {
            CheckAuthorization();
            var postResponse = GetRequest($"{address}posts/group/{name}?page={page}&perpage={perpage}");
            if (postResponse.StatusCode != System.Net.HttpStatusCode.OK && postResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return (null, 0);
            if (postResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                postResponse = GetRequest($"{address}posts/group/{name}?page={page}&perpage={perpage}");
            }
            string jsonResponse = postResponse.Content.ReadAsStringAsync().Result;
            var posts = JsonConvert.DeserializeObject<(List<Post>, int)>(jsonResponse);
            return posts;
        }

        public Post AddPost(Post post)
        {
            CheckAuthorization();
            var response = PostRequest($"{address}posts/post", post);
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                response = PostRequest($"{address}posts/post", post);
            }
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                return post;
            return null;
        }

        public HttpResponseMessage EditPost(int id, string newText)
        {
            CheckAuthorization();
            var response = PutRequest($"{address}posts/post/", id, newText);
            if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                response = PutRequest($"{address}posts/post/", id, newText);
            }
            return response;
        }

        public HttpResponseMessage DeletePost(int id)
        {
            CheckAuthorization();
            var postResponse = DeleteRequest($"{address}posts/post/", id);
            if (postResponse.StatusCode != System.Net.HttpStatusCode.OK && postResponse.StatusCode != System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (postResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                postResponse = DeleteRequest($"{address}posts/post/", id);
            }
            return postResponse;
        }

        public Post Convert(PostModel postModel)
        {
            return new Post { Id = postModel.Id, Text = postModel.Text };
        }

        private void CheckAuthorization()
        {
            if (!Authorized)
            {
                token = Authorize($"{address}posts", PostsCredentials.Login, PostsCredentials.Password);
                Authorized = true;
            }
        }
    }
}
