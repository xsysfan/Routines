﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext context;
        private readonly bool asNoTracking;

        public Repository(DbContext context, bool asNoTracking)
        {
            this.context = context;
            this.asNoTracking = asNoTracking;
        }

        public IQueryable<TEntity> MakeQueryable(Include<TEntity> include)
        {
            var dbSet = context.Set<TEntity>();
            IQueryable<TEntity> query;
            if (asNoTracking)
                query = dbSet.AsNoTracking();
            else
                query = dbSet.AsQueryable();
            query = query.Include(include);
            return query;
        }

        public IReadOnlyCollection<TEntity> List(Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.ToList();
            return list;
        }

        public IReadOnlyCollection<TEntity> List(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.Where(predicate).ToList();
            return list;
        }

        public TEntity Find(Expression<Func<TEntity, bool>> predicate, Include<TEntity> include = null)
        {
            var queryable = MakeQueryable(include);
            var list = queryable.Where(predicate).SingleOrDefault();
            return list;
        }

        public IRepository<TNewBaseEntity> Sprout<TNewBaseEntity>() where TNewBaseEntity : class
        {
            return new Repository<TNewBaseEntity>(this.context, asNoTracking);
        }

        public void Detach(TEntity entity, Include<TEntity> include = null)
        {
            context.Detach(entity, include);
        }

        public void Detach(IEnumerable<TEntity> entities, Include<TEntity> include = null)
        {
            foreach (var entity in entities)
            {
                context.Detach(entity, include);
            }
        }

        public Include<TEntity> AppendModelFields(Include<TEntity> include) 
        {
            return EfCoreExtensions.AppendModelFields(include, context);
        }

        public Include<TEntity> ExtractNavigations(Include<TEntity> include)
        {
            return EfCoreExtensions.ExtractNavigations(include, context);
        }

        public Include<TEntity> ExtractNavigationsAppendKeyLeafs(Include<TEntity> include)
        {
            return EfCoreExtensions.ExtractNavigationsAppendKeyLeafs(include, context);
        }
    }
}
