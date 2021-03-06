﻿using Microsoft.EntityFrameworkCore;
using Posts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posts
{
    public class PostsDbContext : DbContext
    {
        public PostsDbContext(DbContextOptions options) : base(options)
        {
            Initialize();
        }

        protected PostsDbContext()
        {
        }

        public DbSet<Post> Posts { get; set; }

        private void Initialize()
        {
            if (!Posts.Any())
            {
                Posts.Add(new Post { Author = 1, Group = 1, Text = "Very long post."});
                Posts.Add(new Post { Author = 1, Group = 1, Text = "More long than previous." });
                SaveChanges();
            }
        }
    }
}
