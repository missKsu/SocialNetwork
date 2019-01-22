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
        private readonly TokensStorage tokensStorage;

        public PostsController(PostsDbContext dbContext, TokensStorage tokensStorage)
        {
            this.dbContext = dbContext;
            this.tokensStorage = tokensStorage;
        }

        [HttpPost("auth")]
        public string Auth([FromBody]Auth auth)
        {
            if (auth.Login == PostsCredentials.Login && auth.Pass == PostsCredentials.Password)
            {
                var token = Guid.NewGuid().ToString();
                tokensStorage.AddToken(token);
                return token;
            }
            return null;
        }

        public ActionResult<bool> CheckToken(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                return StatusCode(400);
            }
            var token = httpContext.Request.Headers["Authorization"].ToString();
            token = token.Substring(token.IndexOf(' ') + 1);
            var check = tokensStorage.CheckToken(token);
            if (check == AuthorizeResult.NoToken || check == AuthorizeResult.WrongToken)
            {
                return StatusCode(403);
            }
            if (check == AuthorizeResult.TokenExpired)
                return StatusCode(401);
            return true;
        }

        [HttpGet("id/{id}")]
        public ActionResult<Post> GetPostById(int id)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            return dbContext.Posts.FirstOrDefault(p => p.Id == id);
        }

        [HttpGet("author/{author}")]
        public ActionResult<(List<Post>,int)> FindPostsByAuthor(int author, int page, int perpage)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
        public ActionResult<(List<Post>, int)> FindPostsByGroup(int group, int page, int perpage)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
