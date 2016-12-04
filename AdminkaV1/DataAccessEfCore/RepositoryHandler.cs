﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Vse.Routines.Storage;
using Vse.Routines.Storage.EfCore;

namespace Vse.AdminkaV1.DataAccessEfCore
{
    public class RepositoryHandler<TEntity> : IRepositoryHandler<TEntity> 
        where TEntity : class
    {
        readonly AdminkaDbContextHandler adminkaDbContextHandler;
        readonly Func<Exception, List<FieldError>> analyzeException;
        readonly bool noTracking;

        public RepositoryHandler(
            AdminkaDbContextHandler adminkaDbContextHandler,
            Func<Exception, List<FieldError>> analyzeException,
            bool noTracking
            )
        {
            this.adminkaDbContextHandler = adminkaDbContextHandler;
            this.analyzeException = analyzeException;
            this.noTracking = noTracking;
        }

        public void Handle(Action<IRepository<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context) => action(new Repository<TEntity>(container, noTracking)));
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context) => func(new Repository<TEntity>(container, noTracking)));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context) =>
                {
                    output = func(new Repository<TEntity>(container, noTracking));
                });
                return output;
            });
        }

        public TOutput Handle<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func)
        {
            return adminkaDbContextHandler.Handle((container, context, setAudit) => func(
                new Repository<TEntity>(container, false),
                new Storage<TEntity>(container, analyzeException, setAudit)
                ));
        }

        public Task<TOutput> HandleAsync<TOutput>(Func<IRepository<TEntity>, IStorage<TEntity>, TOutput> func)
        {
            return Task.Run(() =>
            {
                var output = default(TOutput);
                adminkaDbContextHandler.Handle((container, context, setAudit) =>
                {
                    output = func(
                        new Repository<TEntity>(container, noTracking),
                        new Storage<TEntity>(container,  analyzeException, setAudit)
                        );
                });
                return output;
            });
        }

        public void Handle(Action<IRepository<TEntity>, IStorage<TEntity>> action)
        {
            adminkaDbContextHandler.Handle((container, context, setAudit) => action(
                new Repository<TEntity>(container, false),
                new Storage<TEntity>(container,  analyzeException, setAudit)
                ));
        }
    }
}
