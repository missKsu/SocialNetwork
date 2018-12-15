using FluentAssertions;
using Groups.Entities;
using Moq;
using SocialNetwork.Api;
using SocialNetwork.Controllers;
using SocialNetwork.Models.Groups;
using SocialNetwork.Models.Users;
using System;
using System.Collections.Generic;
using System.Text;
using Users.Entities;
using Xunit;

namespace SocialNetwork.Test.Controllers
{
    public class GatewayControllerTests
    {
        [Fact]
        public void FindValidGroupByName()
        {
            UserModel user = new UserModel { Name = "User"};
            var usersApi = Mock.Of<UsersApi>(api => api.FindUsersById(It.IsAny<int>()) == user);

            Group group = new Group { Name = "Group", Creator = 1, Description = "public" };
            var groupsApi = Mock.Of<GroupsApi>(api => api.FindGroupByName(It.IsAny<string>()) == group);

            var postsApi = Mock.Of<PostsApi>();
            var permissionsApi = Mock.Of<PermissionsApi>();

            var controller = new GatewayController(usersApi, groupsApi, postsApi, permissionsApi);
            var result = controller.FindGroupByName("Group");

            // assert
            var expected = new GroupModel { Name = "Group", Creator = "User", Description = "public"};
            result.Value.Should().BeEquivalentTo(expected);
        }
    }
}
