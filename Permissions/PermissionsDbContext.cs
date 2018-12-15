using Microsoft.EntityFrameworkCore;
using Permissions.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Permissions
{
    public class PermissionsDbContext : DbContext
    {
        public PermissionsDbContext(DbContextOptions options) : base(options)
        {
            Initialize();
        }


        protected PermissionsDbContext()
        {
        }

        public DbSet<Permission> Permissions { get; set; }

        private void Initialize()
        {
            if (Permissions.Count() == 0)
            {
                Permissions.Add(new Permission { SubjectType = Subject.User, SubjectId = 1, ObjectType = Entities.Object.Group, ObjectId = 1, Operation = Operation.Admin });
                Permissions.Add(new Permission { SubjectType = Subject.User, SubjectId = 2, ObjectType = Entities.Object.Group, ObjectId = 1, Operation = Operation.Read });
                Permissions.Add(new Permission { SubjectType = Subject.User, SubjectId = 2, ObjectType = Entities.Object.Group, ObjectId = 2, Operation = Operation.Admin });
                Permissions.Add(new Permission { SubjectType = Subject.User, SubjectId = 1, ObjectType = Entities.Object.Group, ObjectId = 2, Operation = Operation.Read });
                Permissions.Add(new Permission { SubjectType = Subject.User, SubjectId = 3, ObjectType = Entities.Object.Group, ObjectId = 3, Operation = Operation.Admin });
                SaveChanges();
            }
        }
    }
}
