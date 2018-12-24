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
        public (List<Post>,int) FindPostsByAuthor(int author, int page, int perpage)
        {
            var result = dbContext.Posts.Where(p => p.Author == author);
            result = result.OrderByDescending(n => n.Text);
            int maxPage = result.Count() / perpage + (result.Count() % perpage == 0 ? 0 : 1);
            if (page != 0)
            {
                result = result.Skip(perpage * (page-1));
            }
            result = result.Take(perpage);

            return (result.ToList(),maxPage);
        }

        [HttpGet("group/{group}")]
        public (List<Post>, int) FindPostsByGroup(int group, int page, int perpage)
        {
            var result = dbContext.Posts.Where(p => p.Group == group);
            result = result.OrderByDescending(n => n.Text);
            int maxPage = result.Count() / perpage + (result.Count() % perpage == 0 ? 0 : 1);
            if (page != 0)
            {
                result = result.Skip(perpage * (page - 1));
            }
            result = result.Take(perpage);

            return (result.ToList(), maxPage);
        }

        [HttpPut("post/{id}")]
        public ActionResult UpdatePost(int id, string newText)
        {
            var post = dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                post.Text = newText;
                dbContext.Posts.Update(post);
                return StatusCode(200);
            }
            return StatusCode(400);
        }

        [HttpPost("post")]
        public ActionResult AddPost([FromBody]Post post)
        {
            if (post.Author != -1 && post.Group != -1 && post.Text != "")
            {
                dbContext.Posts.Add(post);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(404);
        }

        [HttpDelete("post/{id}")]
        public ActionResult DeletePost(int id)
        {
            var post = dbContext.Posts.FirstOrDefault(p => p.Id == id);
            if (post != null)
            {
                dbContext.Posts.Remove(post);
                dbContext.SaveChanges();
                return StatusCode(200);
            }
            return StatusCode(422);
        }
    }
}
