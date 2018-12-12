using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Api;
using SocialNetwork.Models.Groups;
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

        public GatewayController(UsersApi usersApi, GroupsApi groupsApi, PostsApi postsApi)
        {
            this.usersApi = usersApi;
            this.groupsApi = groupsApi;
            this.postsApi = postsApi;
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
            if(creator == 0)
            {
                return null;
            }
            var body = groupsApi.Convert(group);
            body.Creator = creator;
            var response = groupsApi.AddGroup(body);
            return group;
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
                posts.Add(new PostModel { Author = author.Name, Text = post.Text, Group = group });
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
            var result = groupsApi.DeleteGroup(groupModel.GroupName);
            return RedirectToAction(nameof(GetAllGroupsByIf));
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
            var result = groupsApi.EditGroup(groupModel.Name, groupModel.NewName, groupModel.NewDescription);
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [HttpGet("posts")]
        public ActionResult<AllPostsModel> PartOfPosts(string name, int page)
        {
            var result = GetPostsByGroup(name,page+1,1);
            var posts = new AllPostsModel { Posts = result.Value.Content, page = result.Value.Page };
            return View(posts);
        }
    }
}
