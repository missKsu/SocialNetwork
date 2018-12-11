using FluentAssertions;
using Moq;
using Posts.Controllers;
using Posts.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestsCommon;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Posts.Test.Controllers
{
    public class PostsControllerTests
    {
        [Fact]
        public void ReturnsValidPostById()
        {
            // arrange
            Post post = CreatePost(1, 1, 1, "Very long post.");
            var controller = CreateControllerWithPosts(post);

            // act
            var result = controller.GetPostById(1);

            // assert
            result.Value.Should().Be(post);
        }

        [Fact]
        public void ReturnsNullIfNoPostById()
        {
            // arrange
            var controller = CreateControllerWithPosts();

            // act
            var result = controller.GetPostById(1);

            // assert
            result.Value.Should().BeNull();
        }

        [Fact]
        public void ReturnsValidPostsByAuthor()
        {
            // arrange
            Post post1 = CreatePost(1, 1, 1, "post1");
            Post post2 = CreatePost(1, 1, 2, "post2");
            Post post3 = CreatePost(1, 2, 1, "post3");
            var controller = CreateControllerWithPosts(post1, post2, post3);

            // act
            var result = controller.FindPostsByAuthor(1,1,2);

            // assert
            var expected = new List<Post>(new Post[] { post1, post2 });
            result.Item1.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReturnsEmptyIfNoPostsByAuthor()
        {
            // arrange
            var controller = CreateControllerWithPosts();

            // act
            var result = controller.FindPostsByAuthor(1,1,2);

            // assert
            result.Item1.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsValidPostsByGroup()
        {
            // arrange
            Post post1 = CreatePost(1, 1, 1, "post1");
            Post post2 = CreatePost(1, 1, 2, "post2");
            Post post3 = CreatePost(1, 2, 1, "post3");
            var controller = CreateControllerWithPosts(post1, post2, post3);

            // act
            var result = controller.FindPostsByGroup(1,1,2);

            // assert
            var expected = new List<Post>(new Post[] { post1, post3 });
            result.Item1.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReturnsEmptyIfNoPostsByGroup()
        {
            // arrange
            var controller = CreateControllerWithPosts();

            // act
            var result = controller.FindPostsByGroup(1,1,1);

            // assert
            result.Item1.Should().BeEmpty();
        }

        [Fact]
        public void UpdateExistPost()
        {
            Post post = CreatePost(1, 1, 1, "This is a post.");
            var controller = CreateControllerWithPosts(post);

            var result = controller.UpdatePost(1, "No post");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(200);
        }

        [Fact]
        public void UpdateNoExistPost()
        {
            var controller = CreateControllerWithPosts();

            var result = controller.UpdatePost(1, "No post");

            var code = result as StatusCodeResult;
            code.Should().NotBeNull();
            code.StatusCode.Should().Be(400);
        }

        private PostsController CreateControllerWithPosts(params Post[] posts)
        {
            var dbContext = MockCreator.CreateDbContextFromCollection(
                dbSet => Mock.Of<PostsDbContext>(db => db.Posts == dbSet),
                posts);
            return new PostsController(dbContext);
        }

        private Post CreatePost(int id, int author, int group, string text)
        {
            return new Post { Id = id, Author = author, Group = group, Text = text };
        }
    }
}
