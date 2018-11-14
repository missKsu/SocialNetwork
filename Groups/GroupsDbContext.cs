using Groups.Entities;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Groups
{
    public class GroupsDbContext : DbContext
    {
        public GroupsDbContext(DbContextOptions options) : base(options)
        {
            Initialize();
        }

        protected GroupsDbContext()
        {
        }

        public DbSet<Group> Groups { get; set; }

        private void Initialize()
        {
            if (!Groups.Any())
            {
                Groups.Add(new Group { Name = "TestGroup", Creator = 1, Description = "element1" });
                Groups.Add(new Group { Name = "TestGroup2", Creator = 1, Description = "element2" });
                Groups.Add(new Group { Name = "TestGroup3", Creator = 2, Description = "element1" });
                SaveChanges();
            }
        }
    }
}
