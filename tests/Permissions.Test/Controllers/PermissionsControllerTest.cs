using FluentAssertions;
using Moq;
using Permissions.Controllers;
using Permissions.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using TestsCommon;
using Xunit;

namespace Permissions.Test.Controllers
{
    public class PermissionsControllerTest
    {
        [Fact]
        public void ReturnsValidUserPermissionsById()
        {
            // arrange
            Permission permission1 = CreatePermission(1, Subject.User, 1, Entities.Object.Group, 1, Operation.Read);
            Permission permission2 = CreatePermission(2, Subject.User, 1, Entities.Object.Group, 1, Operation.Write);
            var controller = CreateControllerWithPermissions(permission1,permission2);

            // act
            var result = controller.GetUserPermissionsById(1);

            // assert
            var expected = new List<Permission>(new Permission[] { permission1, permission2 });
            result.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void ReturnsNullIfNoUserPermissionsById()
        {
            // arrange
            var controller = CreateControllerWithPermissions();

            // act
            var result = controller.GetUserPermissionsById(1);

            // assert
            result.Value.Should().BeEmpty();
        }

        [Fact]
        public void ReturnsValidPermissionsByObjectAndOperation()
        {
            // arrange
            Permission permission1 = CreatePermission(1, Subject.User, 1, Entities.Object.Group, 1, Operation.Read);
            Permission permission2 = CreatePermission(2, Subject.User, 1, Entities.Object.Group, 1, Operation.Write);
            Permission permission3 = CreatePermission(2, Subject.User, 1, Entities.Object.Group, 2, Operation.Write);
            var controller = CreateControllerWithPermissions(permission1, permission2, permission3);

            // act
            var result = controller.GetUserPermissionsByObjectAndOperation(Entities.Object.Group,1,Operation.Write);

            // assert
            var expected = new List<Permission>(new Permission[] { permission2 });
            result.Value.Should().BeEquivalentTo(expected);
        }

        private PermissionsController CreateControllerWithPermissions(params Permission[] permissions)
        {
            var dbContext = MockCreator.CreateDbContextFromCollection(
                dbSet => Mock.Of<PermissionsDbContext>(db => db.Permissions == dbSet),
                permissions);
            return new PermissionsController(dbContext);
        }

        private Permission CreatePermission(int id, Subject subjectType, int subjectId, Entities.Object objectType, int objectId, Operation operation)
        {
            return new Permission { Id = id, SubjectType = subjectType, SubjectId = subjectId, ObjectType = objectType, ObjectId = objectId, Operation = operation};
        }
    }
}
