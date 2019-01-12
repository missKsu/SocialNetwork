using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Users.Entities;

namespace Users
{
    public class UsersDbContext : DbContext
    {
        public UsersDbContext(DbContextOptions options) : base(options)
        {
            Initialize();
        }

        protected UsersDbContext()
        {
        }

        public DbSet<User> Users { get; set; }

        private void Initialize()
        {
            if (Users.Count() == 0)
            {
                Users.Add(new User { Name = "TESTUSER1", Password = "TESTUSER" });
                Users.Add(new User { Name = "TestUser2" });
                SaveChanges();
            }
        }
    }
}
