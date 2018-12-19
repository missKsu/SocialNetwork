using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Mvc;
using Permissions.Entities;
using Posts.Entities;
using SocialNetwork.Api;
using SocialNetwork.Models;
using SocialNetwork.Models.Groups;
using SocialNetwork.Models.Permissions;
using SocialNetwork.Models.Posts;
using SocialNetwork.Pagination;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    public class GatewayController : Controller
    {
        private readonly UsersApi usersApi;
        private readonly GroupsApi groupsApi;
        private readonly PostsApi postsApi;
        private readonly PermissionsApi permissionsApi;

        public GatewayController(UsersApi usersApi, GroupsApi groupsApi, PostsApi postsApi, PermissionsApi permissionsApi)
        {
            this.usersApi = usersApi;
            this.groupsApi = groupsApi;
            this.postsApi = postsApi;
            this.permissionsApi = permissionsApi;
        }

        [HttpGet("groups/{name}")]
        public ActionResult<GroupModel> FindGroupByName(string name)
        {
            var group = groupsApi.FindGroupByName(name);
            if (group == null)
            {
                return StatusCode(404);
            }
            var userName = usersApi.FindUsersById(group.Creator);
            if (userName == null)
            {
                return StatusCode(404);
            }
            return new GroupModel { Name = group.Name, Creator = userName.Name, Description = group.Description };
        }

        [HttpGet("groups/id/{id}")]
        public ActionResult<GroupModel> FindGroupById(int id)
        {
            var group = groupsApi.FindGroupById(id);
            var userName = usersApi.FindUsersById(group.Creator);
            return new GroupModel { Name = group.Name, Creator = userName.Name, Description = group.Description };
        }

        [HttpPost("groups")]
        public ActionResult<GroupModel> AddGroup(GroupModel group)
        {
            var creator = usersApi.FindIdUserByName(group.Creator);
            if(creator == -1)
            {
                return null;
            }
            var body = groupsApi.Convert(group);
            body.Creator = creator;
            var response = groupsApi.AddGroup(body);
            return group;
        }

        [HttpPost("posts")]
        public ActionResult<PostModel> AddPost(PostModel post)
        {
            var creator = usersApi.FindIdUserByName(post.Author);
            if (creator == -1)
            {
                return null;
            }
            var body = postsApi.Convert(post);
            body.Author = creator;
            var response = postsApi.AddPost(body);
            return post;
        }

        [HttpGet("groups")]
        public ActionResult<List<GroupModel>> GetAllGroups()
        {
            var result = groupsApi.GetAllGroups();
            var groups = new List<GroupModel> { };
            if (result != null)
            {
                foreach(var group in result)
                {
                    var creator = usersApi.FindUsersById(group.Creator);
                    if (creator != null)
                        groups.Add(new GroupModel { Name = group.Name, Description = group.Description, Creator = creator.Name });
                }
            }
            return groups;
        }

        [HttpGet("posts/author/{author}")]
        public ActionResult<PaginatedList<PostModel>> GetPostsByAuthor(string author, int page, int perpage)
        {
            var posts = new List<PostModel> { };
            var authorId = usersApi.FindIdUserByName(author);
            if (authorId == 0)
                return new PaginatedList<PostModel>(posts,0,0,0);
            if (perpage == 0)
                perpage = 50;
            var result = postsApi.GetPostsByAuthor(authorId, page, perpage);
            if (result.Item1 == null)
                return new PaginatedList<PostModel>(posts, 0, 0, 0);
            foreach (var post in result.Item1)
            {
                var group = groupsApi.FindGroupById(post.Group);
                posts.Add(new PostModel { Author = author, Text = post.Text, Group = group.Name});
            }
            return new PaginatedList<PostModel>(posts,perpage,page,result.Item2);
        }

        [HttpGet("posts/group/{group}")]
        public ActionResult<PaginatedList<PostModel>> GetPostsByGroup(string group, int page, int perpage)
        {
            var posts = new List<PostModel> { };
            var groupId = groupsApi.FindGroupByName(group);
            if (groupId == null)
                return new PaginatedList<PostModel>(posts, 0, 0, 0);
            if (perpage == 0)
                perpage = 50;
            var result = postsApi.GetPostsByGroup(groupId.Id, page, perpage);
            if (result.Item1 == null)
                return new PaginatedList<PostModel>(posts, 0, 0, 0);
            foreach (var post in result.Item1)
            {
                var author = usersApi.FindUsersById(post.Author);
                posts.Add(new PostModel { Author = author.Name, Text = post.Text, Group = group, Id = post.Id });
            }
            return new PaginatedList<PostModel>(posts, perpage, page, result.Item2);
        }

        [HttpGet("if/groups")]
        public ActionResult GetAllGroupsByIf()
        {
            var result = GetAllGroups();
            var model = new AllGroupsModel { Groups = result.Value};

            return View(model);
        }

        [HttpPut("groups/merge/{group}")]
        public ActionResult<GroupModel> MergeOneGroupWithAnother(string group, string with)
        {

            return null;
        }

        [HttpGet("new")]
        public ActionResult NewGroup()
        {
            return View();
        }

        [HttpGet("newpost/{group}")]
        public ActionResult NewPost(string group)
        {
            return View(new PostModel { Group = group});
        }

        [HttpPost("addpostbyif")]
        public ActionResult<GroupModel> AddPostByIf(PostModel postModel)
        {
            var author = usersApi.FindIdUserByName(postModel.Author);
            var group = groupsApi.FindGroupByName(postModel.Group);
            if (author == -1)
                return RedirectToAction(nameof(MessagePage), new { message = "No such user!" });
            var permission = permissionsApi.GetPermissionForUserByGroup(author, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "Have no information about permission for this group!" });
            if (permission.Operation != Operation.Admin && permission.Operation != Operation.Write)
                return RedirectToAction(nameof(MessagePage), new { message = "Haven't  permission!" });
            var body = new Post {Author = author, Group = group.Id, Text = postModel.Text };
            var result = postsApi.AddPost(body);
            return RedirectToAction(nameof(PartOfPosts),new { name = group.Name, page = 0});
        }

        [HttpPost("addbyif")]
        public ActionResult<GroupModel> AddGroupByIf(GroupModel groupModel)
        {
            var result = AddGroup(groupModel);
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [HttpGet("delete")]
        public ActionResult DeleteGroupIf(string name)
        {
            var group = new DeleteGroupModel { GroupName = name };
            return View(group);
        }

        [HttpPost("deletebyif")]
        public ActionResult<GroupModel> DeleteGroupByIf(DeleteGroupModel groupModel)
        {
            var creator = usersApi.FindIdUserByName(groupModel.Creator);
            var group = groupsApi.FindGroupByName(groupModel.GroupName);
            if (creator == -1)
                return RedirectToAction(nameof(MessagePage), new { message = "No such user!" });
            var permission = permissionsApi.GetPermissionForUserByGroup(creator, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
            if (permission.Operation != Operation.Admin)
                return RedirectToAction(nameof(MessagePage), new { message = "No admin permission!" });
            var result = groupsApi.DeleteGroup(groupModel.GroupName);
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [HttpGet("message/{message}")]
        public ActionResult MessagePage(string message)
        {
            return View("MessagePage",message);
        }

        [HttpGet("edit")]
        public ActionResult EditGroupIf(string name)
        {
            var group = groupsApi.FindGroupByName(name);
            var modifiedGroup = new EditGroupModel { Name = group.Name, NewName = group.Name, Creator = group.Creator, Description = group.Description, NewDescription = group.Description};
            return View(modifiedGroup);
        }

        [HttpPost("editbyif")]
        public ActionResult<GroupModel> EditGroupByIf(EditGroupModel groupModel)
        {
            var group = groupsApi.FindGroupByName(groupModel.Name);
            var permission = permissionsApi.GetPermissionForUserByGroup(groupModel.Creator, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
            if (permission.Operation != Operation.Admin)
                return RedirectToAction(nameof(MessagePage), new { message = "No permission!" });
            var result = groupsApi.EditGroup(groupModel.Name, groupModel.NewName, groupModel.NewDescription);
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [HttpGet("posts")]
        public ActionResult<AllPostsModel> PartOfPosts(string name, int page)
        {
            var result = GetPostsByGroup(name,page+1,10);
            var posts = new AllPostsModel { Posts = result.Value.Content, page = result.Value.Page, GroupName = name };
            if (posts.Posts.Count() == 0)
                posts.message = "No posts! You can be first.";
            return View(posts);
        }

        [HttpGet("posts/delete")]
        public ActionResult DeletePostIf(int id, string group)
        {
            var post = new DeletePostModel { Id = id, Group = group };
            return View(post);
        }

        [HttpPost("posts/deletebyif")]
        public ActionResult<PostModel> DeletePostByIf(DeletePostModel postModel)
        {
            var creator = usersApi.FindIdUserByName(postModel.Creator);
            if (creator == -1)
                return RedirectToAction(nameof(MessagePage), new { message = "No such user!" });
            var post = postsApi.GetPostById(postModel.Id);
            if (post == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No such post!" });
            if (post.Author != creator)
            {
                var permission = permissionsApi.GetPermissionForUserByGroup(creator, post.Group);
                if (permission == null)
                    return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
                if (permission.Operation != Operation.Admin)
                    return RedirectToAction(nameof(MessagePage), new { message = "No permission!" });
            }
            var result = postsApi.DeletePost(postModel.Id);
            return RedirectToAction(nameof(PartOfPosts), new { name = postModel.Group, page = 0 });
        }

        [HttpGet("posts/edit")]
        public ActionResult EditPostIf(EditPostModel postModel)
        {
            var post = postsApi.GetPostById(postModel.Id);
            postModel.Text = post.Text;
            postModel.NewText = post.Text;
            return View(postModel);
        }

        [HttpPost("posts/editbyif")]
        public ActionResult<PostModel> EditPostByIf(EditPostModel postModel)
        {
            var group = groupsApi.FindGroupByName(postModel.Group);
            var creator = usersApi.FindIdUserByName(postModel.Creator);
            var permission = permissionsApi.GetPermissionForUserByGroup(creator, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
            if (permission.Operation != Operation.Admin || creator != group.Creator)
                return RedirectToAction(nameof(MessagePage), new { message = "No permission!" });
            var result = postsApi.EditPost(postModel.Id, postModel.NewText);
            return RedirectToAction(nameof(PartOfPosts), new { name = postModel.Group, page = 0 });
        }
    }
}
