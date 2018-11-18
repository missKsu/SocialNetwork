using Groups.Entities;
using Groups.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TestsCommon;
using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace Groups.Test.Controllers
{
    public class GroupsControllerTests
    {
        [Fact]
        public void ReturnsValidGroupByName()
        {
            // arrange
            Group group = CreateGroup(1, "Name",1, "element1");
            var controller = CreateControllerWithGroups(group);

            // act
            var result = controller.FindGroupByName("Name");

            // assert
            result.Value.Should().Be(group);
        }

        [Fact]
        public void ReturnsNullIfNoGroupByName()
        {
            // arrange
            var controller = CreateControllerWithGroups();

            // act
            var result = controller.FindGroupByName("Name");

            // assert
            result.Value.Should().BeNull();
        }

        [Fact]
        public void AddValidGroup()
        {
            var controller = CreateControllerWithGroups();

            Group group = CreateGroup(1, "Name", 1, "element1");
            var result = controller.AddGroup(group);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void AddGroupWithNoName()
        {
            var controller = CreateControllerWithGroups();

            Group group = CreateGroup(1, "", 1, "element1");
            var result = controller.AddGroup(group);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(404);
        }

        [Fact]
        public void AddGroupWithNoCreator()
        {
            var controller = CreateControllerWithGroups();

            Group group = CreateGroup(1, "Name", 0, "element1");
            var result = controller.AddGroup(group);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(404);
        }

        [Fact]
        public void ReturnsValidGroupsByDescription()
        {
            // arrange
            Group group1 = CreateGroup(1, "Group1", 1, "element1");
            Group group2 = CreateGroup(2, "Group2", 1, "element2");
            Group group3 = CreateGroup(3, "Group3", 2, "element1");
            var controller = CreateControllerWithGroups(group1,group2,group3);

            // act
            var result = controller.FindGroupsByDescription("element1");

            // assert
            var expected = new List<Group>(new Group[] { group1, group3 });
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReturnsNullIfNoGroupsWithNeededDescription()
        {
            // arrange
            var controller = CreateControllerWithGroups();

            // act
            var result = controller.FindGroupsByDescription("element1");

            // assert
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsValidGroupsByCreator()
        {
            // arrange
            Group group1 = CreateGroup(1, "Group1", 1, "element1");
            Group group2 = CreateGroup(2, "Group2", 1, "element2");
            Group group3 = CreateGroup(3, "Group3", 2, "element1");
            var controller = CreateControllerWithGroups(group1, group2, group3);

            // act
            var result = controller.FindGroupsByCreator(1);

            // assert
            var expected = new List<Group>(new Group[] { group1, group2 });
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReturnsNullIfNoGroupsWithNeededCreator()
        {
            // arrange
            var controller = CreateControllerWithGroups();

            // act
            var result = controller.FindGroupsByCreator(1);

            // assert
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public void UpdateExistGroupByValidName()
        {
            Group group = CreateGroup(1, "Group", 1, "element1");
            var controller = CreateControllerWithGroups(group);

            var result = controller.UpdateGroup("Group", "MainGroup", "");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void UpdateExistGroupByValidDescription()
        {
            Group group = CreateGroup(1, "Group", 1, "element1");
            var controller = CreateControllerWithGroups(group);

            var result = controller.UpdateGroup("Group", "", "element2");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void UpdateExistGroupByNoParam()
        {
            Group group = CreateGroup(1, "Group", 1, "element1");
            var controller = CreateControllerWithGroups(group);

            var result = controller.UpdateGroup("Group", "", "");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(422);
        }

        [Fact]
        public void UpdateNoExistGroup()
        {
            var controller = CreateControllerWithGroups();

            var result = controller.UpdateGroup("Group", "Name", "element");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }

        [Fact]
        public void DeleteExistGroup()
        {
            Group group = CreateGroup(1, "Group1", 1, "element1");
            var controller = CreateControllerWithGroups(group);

            var result = controller.DeleteGroup("Group1");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void DeleteNoExistGroup()
        {
            var controller = CreateControllerWithGroups();

            var result = controller.DeleteGroup("Group1");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(422);
        }

        private GroupsController CreateControllerWithGroups(params Group[] groups)
        {
            var dbContext = MockCreator.CreateDbContextFromCollection(
                dbSet => Mock.Of<GroupsDbContext>(db => db.Groups == dbSet),
                groups);
            return new GroupsController(dbContext);
        }

        private Group CreateGroup(int id, string name, int creator, string description)
        {
            return new Group { Id = 1, Name = name, Creator = creator, Description = description };
        }
    }
}
