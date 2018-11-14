using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Posts.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Posts.Controllers
{
    [Route("posts")]
    public class PostsController : Controller
    {
        private readonly PostsDbContext dbContext;

        public PostsController(PostsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet("id/{id}")]
        public ActionResult<Post> GetPostById(int id)
        {
            //LINQ
            return dbContext.Posts.FirstOrDefault(p => p.Id == id);
        }

        [HttpGet("author/{author}")]
        public ActionResult<List<Post>> FindPostsByAuthor(int author)
        {
            var result = dbContext.Posts.Where(p => p.Author == author);
            return result.Select(s => new Post { Id = s.Id, Author = s.Author, Group = s.Group, Text = s.Text }).ToList();
        }

        [HttpGet("group/{group}")]
        public ActionResult<List<Post>> FindPostsByGroup(int group)
        {
            var result = dbContext.Posts.Where(p => p.Group == group);
            return result.Select(s => new Post { Id = s.Id, Author = s.Author, Group = s.Group, Text = s.Text }).ToList();
        }

        [HttpPut("post/{id}")]
        public ActionResult UpdatePost(int id, string text)
        {
            var post = dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                post.Text = text;
                dbContext.Posts.Update(post);
                return StatusCode(200);
            }
            return StatusCode(400);
        }
    }
}
