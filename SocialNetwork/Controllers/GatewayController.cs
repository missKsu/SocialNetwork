using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Groups.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Permissions.Entities;
using Posts.Entities;
using SocialNetwork.Api;
using SocialNetwork.Models;
using SocialNetwork.Models.Groups;
using SocialNetwork.Models.Permissions;
using SocialNetwork.Models.Posts;
using SocialNetwork.Pagination;
using SocialNetwork.Identity;
using SocialNetwork.Infrastructure;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialNetwork.Controllers
{
    public class GatewayController : Controller
    {
        private readonly UsersApi usersApi;
        private readonly GroupsApi groupsApi;
        private readonly PostsApi postsApi;
        private readonly PermissionsApi permissionsApi;
        private readonly TokensStorage tokensStorage;

        private bool interfaceOperation = false;

        public GatewayController(UsersApi usersApi, GroupsApi groupsApi, PostsApi postsApi, PermissionsApi permissionsApi, TokensStorage tokensStorage)
        {
            this.usersApi = usersApi;
            this.groupsApi = groupsApi;
            this.postsApi = postsApi;
            this.permissionsApi = permissionsApi;
            this.tokensStorage = tokensStorage;
        }

        public ActionResult<bool> CheckToken(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            if (!interfaceOperation)
            {
                if (!httpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    return StatusCode(403, "Without authorization in headers you can't get response");
                }
                var token = httpContext.Request.Headers["Authorization"].ToString();
                token = token.Substring(token.IndexOf(' ') + 1);
                var check = tokensStorage.CheckToken(token);
                if (check == SocialNetwork.Identity.AuthorizeResult.NoToken || check == SocialNetwork.Identity.AuthorizeResult.WrongToken)
                {
                    return StatusCode(403, "Without authorization you can't get response");
                }
                if (check == SocialNetwork.Identity.AuthorizeResult.TokenExpired)
                    return RedirectToAction("OAuthReLogin", "Accounts", new { token });
                return true;
            }
            return true;
        }

        private ActionResult WriteAnswer(int code)
        {
            return StatusCode(code,"server are not allowed. Try later");
        }

        [HttpGet("groups/{name}")]
        public ActionResult<GroupModel> FindGroupByName(string name)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var groupResponse = groupsApi.FindGroupByName(name);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
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
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var group = groupsApi.FindGroupById(id);
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

        [HttpPost("groups")]
        public ActionResult<GroupModel> AddGroup(GroupModel group)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var creator = usersApi.FindIdUserByName(group.Creator);
            if(creator == -1)
            {
                return null;
            }
            var body = groupsApi.Convert(group);
            body.Creator = creator;
            var response = groupsApi.AddGroup(body);
            if (response == null)
                return null;
            var responsePermission = permissionsApi.AddPermission(new Permission { SubjectType = Subject.User, SubjectId = creator, ObjectType = Permissions.Entities.Object.Group, ObjectId = response.Id, Operation = Operation.Admin});
            return group;
        }

        [HttpPost("posts")]
        public ActionResult<PostModel> AddPost(PostModel post)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
            interfaceOperation = true;
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var result = groupsApi.GetAllGroups();
            var groups = new List<GroupModel> { };
            if (result != null)
            {
                foreach(var group in result)
                {
                    var creator = usersApi.FindUsersById(group.Creator);
                    if (creator != null)
                        groups.Add(new GroupModel { Name = group.Name, Description = group.Description, Creator = creator.Name });
                    else
                        groups.Add(new GroupModel { Name = group.Name, Description = group.Description, Creator = "" });
                }
            }
            return groups;
        }

        [HttpGet("posts/author/{author}")]
        public ActionResult<PaginatedList<PostModel>> GetPostsByAuthor(string author, int page, int perpage)
        {
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

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
            var res = CheckToken(HttpContext);
            if (!res.Value)
                return res.Result;

            var posts = new List<PostModel> { };
            var groupResponse = groupsApi.FindGroupByName(group);
            var groupId = groupResponse.Item2;
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

        [Authorize]
        [HttpGet("if/groups")]
        public ActionResult GetAllGroupsByIf()
        {
            interfaceOperation = true;
            var result = GetAllGroups();
            var model = new AllGroupsModel { Groups = result.Value};

            return View(model);
        }

        [Authorize]
        [HttpGet("if/permissions")]
        public ActionResult GetPermissionsIf()
        {
            interfaceOperation = true;
            return View();
        }

        [Authorize]
        [HttpGet("if/permissions/user")]
        public ActionResult GetUserPermissionsByIf(string creator)
        {
            interfaceOperation = true;
            creator = User.Identities.First(i => i.IsAuthenticated).Name;
            var user = usersApi.FindIdUserByName(creator);
            var resultUser = permissionsApi.GetUserPermissionsById(user);
            var resultForUser = permissionsApi.GetEditablePermissionsByUserId(user);
            return View(new AllPermissionsForUser { UserPermissions = Convert(resultUser), EditablePermissions = Convert(resultForUser)});
        }

        [HttpPut("groups/merge/{group}")]
        public ActionResult<GroupModel> MergeOneGroupWithAnother(string group, string with)
        {
            Repeater.EnqueueJob(() =>
            {
                var groupResponse = groupsApi.FindGroupByName(group);
                if (groupResponse.Item2 is null)
                    return false;
                var group1 = groupResponse.Item2;
                groupResponse = groupsApi.FindGroupByName(with);
                if (groupResponse.Item2 is null)
                    return false;
                var group2 = groupResponse.Item2;
                var resultGroup = groupsApi.DeleteGroup(group2.Name);
                if (!resultGroup.IsSuccessStatusCode)
                    return false;
                Repeater.EnqueueJob(() =>
                {
                    var resultPosts = postsApi.EditPostGroup(group2.Id, group1.Id);
                    return resultPosts.IsSuccessStatusCode;
                }, 5);
                return true;
            }, 5);
            return StatusCode(200);
        }

        [HttpPut("groups/ext/{group}")]
        public ActionResult<GroupModel> AddExtGroup(string group, string ext)
        {
            var groupResponse = groupsApi.FindGroupByName(group);
            var group1 = groupResponse.Item2;
            var updateResponse = groupsApi.EditGroup(group1.Name, ext + " " + group1.Name, group1.Description);
            if (updateResponse.StatusCode != System.Net.HttpStatusCode.OK)
                return StatusCode(400);
            var postResponse = postsApi.EditExtPost(group1.Id,ext);
            if (updateResponse.StatusCode == System.Net.HttpStatusCode.OK)
                return StatusCode(200);
            updateResponse = groupsApi.EditGroup(ext + " " + group1.Name, group1.Name, group1.Description);
            return StatusCode(400);
        }

        [Authorize]
        [HttpGet("new")]
        public ActionResult NewGroup()
        {
            interfaceOperation = true;
            return View();
        }

        [Authorize]
        [HttpGet("permissions/new")]
        public ActionResult NewPermission()
        {
            interfaceOperation = true;
            return View();
        }

        [Authorize]
        [HttpGet("newpost/{group}")]
        public ActionResult NewPost(string group)
        {
            interfaceOperation = true;
            return View(new PostModel { Group = group});
        }

        [Authorize]
        [HttpPost("addpostbyif")]
        public ActionResult<GroupModel> AddPostByIf(PostModel postModel)
        {
            interfaceOperation = true;
            var author = usersApi.FindIdUserByName(postModel.Author);
            var groupResponse = groupsApi.FindGroupByName(postModel.Group);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
            }
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

        [Authorize]
        [HttpPost("addbyif")]
        public ActionResult<GroupModel> AddGroupByIf(GroupModel groupModel)
        {
            interfaceOperation = true;
            var result = AddGroup(groupModel);
            if (result == null)
                return RedirectToAction(nameof(MessagePage), new { message = "Not add!" });
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [Authorize]
        [HttpPost("permissions/addbyif")]
        public ActionResult<GroupModel> AddPermissionByIf(PermissionModel permissionModel)
        {
            interfaceOperation = true;
            var permission = Convert(permissionModel);
            var check = permissionsApi.GetPermissionByFull(permission);
            if (check == null)
                return RedirectToAction(nameof(MessagePage), new { message = "This user has permission!" });
            var result = permissionsApi.AddPermission(permission);
            if (result == null)
                return RedirectToAction(nameof(MessagePage), new { message = "Not add!" });
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [Authorize]
        [HttpGet("delete")]
        public ActionResult DeleteGroupIf(string name)
        {
            interfaceOperation = true;
            var group = new DeleteGroupModel { GroupName = name };
            return View(group);
        }

        [Authorize]
        [HttpPost("deletebyif")]
        public ActionResult<GroupModel> DeleteGroupByIf(DeleteGroupModel groupModel)
        {
            interfaceOperation = true;
            var creator = usersApi.FindIdUserByName(groupModel.Creator);
            var groupResponse = groupsApi.FindGroupByName(groupModel.GroupName);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
            }
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

        [Authorize]
        [HttpGet("message/{message}")]
        public ActionResult MessagePage(string message)
        {
            interfaceOperation = true;
            return View("MessagePage",message);
        }

        [Authorize]
        [HttpGet("edit")]
        public ActionResult EditGroupIf(string name)
        {
            interfaceOperation = true;
            var groupResponse = groupsApi.FindGroupByName(name);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
            }
            var modifiedGroup = new EditGroupModel { Name = group.Name, NewName = group.Name, Creator = group.Creator, Description = group.Description, NewDescription = group.Description};
            return View(modifiedGroup);
        }

        [Authorize]
        [HttpPost("editbyif")]
        public ActionResult<GroupModel> EditGroupByIf(EditGroupModel groupModel)
        {
            interfaceOperation = true;
            var groupResponse = groupsApi.FindGroupByName(groupModel.Name);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
            }
            var permission = permissionsApi.GetPermissionForUserByGroup(groupModel.Creator, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
            if (permission.Operation != Operation.Admin)
                return RedirectToAction(nameof(MessagePage), new { message = "No permission!" });
            var result = groupsApi.EditGroup(groupModel.Name, groupModel.NewName, groupModel.NewDescription);
            return RedirectToAction(nameof(GetAllGroupsByIf));
        }

        [Authorize]
        [HttpGet("posts")]
        public ActionResult<AllPostsModel> PartOfPosts(string name, int page)
        {
            interfaceOperation = true;
            var result = GetPostsByGroup(name,page+1,10);
            var posts = new AllPostsModel { Posts = result.Value.Content, page = result.Value.Page, GroupName = name };
            if (posts.Posts.Count() == 0)
                posts.message = "No posts! You can be first.";
            return View(posts);
        }

        [Authorize]
        [HttpGet("posts/delete")]
        public ActionResult DeletePostIf(int id, string group)
        {
            interfaceOperation = true;
            var post = new DeletePostModel { Id = id, Group = group };
            return View(post);
        }

        [HttpPost("posts/deletebyif")]
        public ActionResult<PostModel> DeletePostByIf(DeletePostModel postModel)
        {
            interfaceOperation = true;
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

        [Authorize]
        [HttpGet("posts/edit")]
        public ActionResult EditPostIf(EditPostModel postModel)
        {
            interfaceOperation = true;
            var post = postsApi.GetPostById(postModel.Id);
            postModel.Text = post.Text;
            postModel.NewText = post.Text;
            return View(postModel);
        }

        [HttpPost("posts/editbyif")]
        public ActionResult<PostModel> EditPostByIf(EditPostModel postModel)
        {
            interfaceOperation = true;
            var groupResponse = groupsApi.FindGroupByName(postModel.Group);
            var group = groupResponse.Item2;
            if (groupResponse.Item1 != 200)
            {
                return WriteAnswer(groupResponse.Item1);
            }
            var creator = usersApi.FindIdUserByName(postModel.Creator);
            var permission = permissionsApi.GetPermissionForUserByGroup(creator, group.Id);
            if (permission == null)
                return RedirectToAction(nameof(MessagePage), new { message = "No information about permission for this group!" });
            if (permission.Operation != Operation.Admin || creator != group.Creator)
                return RedirectToAction(nameof(MessagePage), new { message = "No permission!" });
            var result = postsApi.EditPost(postModel.Id, postModel.NewText);
            return RedirectToAction(nameof(PartOfPosts), new { name = postModel.Group, page = 0 });
        }

        public List<PermissionModel> Convert(List<Permission> permissions)
        {
            interfaceOperation = true;
            var result = new List<PermissionModel> { };
            if (permissions is null)
                return result;
            foreach (Permission permission in permissions)
            {
                var permissionModel = new PermissionModel { };
                switch (permission.ObjectType)
                {
                    case Permissions.Entities.Object.Group:
                        var group = groupsApi.FindGroupById(permission.ObjectId);
                        if (group == null)
                            break;
                        permissionModel.ObjectType = "Group";
                        permissionModel.ObjectName = group.Name;
                        break;
                    case Permissions.Entities.Object.Post:
                        permissionModel.ObjectType = "Post";
                        break;
                    case Permissions.Entities.Object.Permission:
                        break;
                    default:
                        break;
                }
                switch (permission.Operation)
                {
                    case Operation.Write:
                        permissionModel.Operation = "Write";
                        break;
                    case Operation.Read:
                        permissionModel.Operation = "Read";
                        break;
                    case Operation.CantWrite:
                        permissionModel.Operation = "Can't write";
                        break;
                    case Operation.CantRead:
                        permissionModel.Operation = "Can't read";
                        break;
                    case Operation.Admin:
                        permissionModel.Operation = "Admin";
                        break;
                }
                permissionModel.SubjectType = "User";
                permissionModel.SubjectName = usersApi.FindUsersById(permission.SubjectId).Name;
                result.Add(permissionModel);
            }
            return result;
        }

        public Permission Convert (PermissionModel permissionModel)
        {
            var permission = new Permission { };
            switch (permissionModel.ObjectType)
            {
                case "Group":
                    var groupResponse = groupsApi.FindGroupByName(permissionModel.ObjectName);
                    var group = groupResponse.Item2;
                    if (group == null)
                        break;
                    permission.ObjectType = Permissions.Entities.Object.Group;
                    permission.ObjectId = group.Id;
                    break;
                case "Post":
                    permission.ObjectType = Permissions.Entities.Object.Post;
                    permission.ObjectId = Int32.Parse(permissionModel.ObjectName);
                    break;
                default:
                    break;
            }
            permission.SubjectType = Subject.User;
            permission.SubjectId = usersApi.FindIdUserByName(permissionModel.SubjectName);
            switch (permissionModel.Operation)
            {
                case "Write":
                    permission.Operation = Operation.Write;
                    break;
                case "Read":
                    permission.Operation = Operation.Read;
                    break;
                case "Can't write":
                    permission.Operation = Operation.CantWrite;
                    break;
                case "Can't read":
                    permission.Operation = Operation.CantRead;
                    break;
                case "Admin":
                    permission.Operation = Operation.Admin;
                    break;
            }
            return permission;
        }
    }
}
