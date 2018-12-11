using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TestsCommon
{
    public static class MockCreator
    {
        public static TDbContext CreateDbContextFromCollection<TDbContext, TEntity>(
            Func<DbSet<TEntity>, TDbContext> dbContextCreator,  IEnumerable<TEntity> enumerable)
            where TDbContext : DbContext
            where TEntity : class
        {
            return GetDbContext(dbContextCreator, enumerable.ToList());
        }

        private static TDbContext GetDbContext<TDbContext, TEntity>(
            Func<DbSet<TEntity>, TDbContext> dbContextCreator, List<TEntity> entities = null)
            where TDbContext : DbContext
            where TEntity : class
        {
            if (entities == null)
                entities = new List<TEntity>();
            var dbset = GetElements(entities);
            return dbContextCreator(dbset);
        }

        private static DbSet<TEntity> GetElements<TEntity>(List<TEntity> entitites)
            where TEntity : class
        {
            var usersQueryable = entitites.AsQueryable();
            var mockSet = new Mock<DbSet<TEntity>>();
            var mockSetQueryable = mockSet.As<IQueryable<TEntity>>();
            mockSetQueryable.Setup(m => m.Provider).Returns(usersQueryable.Provider);
            mockSetQueryable.Setup(m => m.Expression).Returns(usersQueryable.Expression);
            mockSetQueryable.Setup(m => m.ElementType).Returns(usersQueryable.ElementType);
            mockSetQueryable.Setup(m => m.GetEnumerator()).Returns(entitites.GetEnumerator());
            mockSet.Setup(d => d.Add(It.IsAny<TEntity>())).Callback<TEntity>(u => entitites.Add(u));
            mockSet.Setup(d => d.Remove(It.IsAny<TEntity>())).Callback<TEntity>(u => entitites.Remove(u));
            return mockSet.Object;
        }
    }
}
