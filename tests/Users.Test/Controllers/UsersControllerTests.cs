using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestsCommon;
using Users.Controllers;
using Users.Entities;
using Xunit;

namespace Users.Test.Controllers
{
    public class UsersControllerTests
    {
        [Fact]
        public void ReturnsValidUserById()
        {
            // arrange
            User user = CreateUser(1, "Name");
            var controller = CreateControllerWithUsers(user);

            // act
            var result = controller.GetUserById(1);

            // assert
            result.Value.Should().Be(user);
        }

        [Fact]
        public void ReturnsNullIfNoUserFoundById()
        {
            var controller = CreateControllerWithUsers();

            var result = controller.GetUserById(1);

            result.Value.Should().BeNull();
        }

        [Fact]
        public void ReturnValidUserByName()
        {
            User user = CreateUser(1, "Name");
            var controller = CreateControllerWithUsers(user);

            var result = controller.GetUserByName("Name");

            result.Value.Should().Be(user);
        }

        [Fact]
        public void ReturnNullIfNoUserFoundByName()
        {
            var controller = CreateControllerWithUsers();

            var result = controller.GetUserByName("Name");

            result.Value.Should().BeNull();
        }

        [Fact]
        public void AddValidUser()
        {
            var controller = CreateControllerWithUsers();

            User user = CreateUser(1, "Name");
            var result = controller.AddUser(user);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void AddUserWithNoName()
        {
            var controller = CreateControllerWithUsers();

            User user = CreateUser(0, "");
            var result = controller.AddUser(user);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }
        
        [Fact]
        public void AddUserWithNullName()
        {
            var controller = CreateControllerWithUsers();

            User user = CreateUser(0, null);
            var result = controller.AddUser(user);

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }

        [Fact]
        public void UpdateExistUserByValidName()
        {
            User user = CreateUser(1, "Name");
            var controller = CreateControllerWithUsers(user);
            
            var result = controller.UpdateUser("Name","NewName");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void UpdateNotExistUser()
        {
            var controller = CreateControllerWithUsers();

            var result = controller.UpdateUser("Name", "NewName");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }

        [Fact]
        public void UpdateUserByExistName()
        {
            User user1 = CreateUser(1, "Name");
            User user2 = CreateUser(2, "NewName");
            var controller = CreateControllerWithUsers(user1,user2);

            var result = controller.UpdateUser("Name", "NewName");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }

        [Fact]
        public void DeleteExistUser()
        {
            User user = CreateUser(1, "Name");
            var controller = CreateControllerWithUsers(user);

            var result = controller.DeleteUser("Name");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void DeleteNotExistUser()
        {
            var controller = CreateControllerWithUsers();

            var result = controller.DeleteUser("Name");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(422);
        }

        private UsersController CreateControllerWithUsers(params User[] users)
        {
            var dbContext = MockCreator.CreateDbContextFromCollection(
                dbSet => Mock.Of<UsersDbContext>(db => db.Users == dbSet), 
                users);
            return new UsersController(dbContext);
        }

        private User CreateUser(int id, string name) => new User { Id = id, Name = name };
    }
}
