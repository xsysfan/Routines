﻿using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class OrmStorage<TEntity> : IOrmStorage<TEntity> where TEntity : class
    {
        private readonly DbContext dbContext;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly IAuditVisitor auditVisitor;

        public OrmStorage(
            DbContext dbContext,
            Func<Exception, StorageResult> analyzeException,
            IAuditVisitor auditVisitor=null)
        {
            this.dbContext = dbContext;
            this.analyzeException = analyzeException;
            this.auditVisitor = auditVisitor ?? NoAuditVisitor.Singleton;
        }

        public StorageResult Handle(Action<IBatch<TEntity>> action)
        {
            return HandleAnalyzableException(() => 
                HandleSave(batch => 
                    action(batch)
                )
            );
        }

        public async Task<StorageResult> HandleAsync(Func<IBatch<TEntity>, Task> action)
        {
            return await HandleAnalyzableExceptionAsync(async () => 
                await HandleSaveAsync(async batch => 
                     await action(batch)
                )
            );
        }

        public StorageResult HandleAnalyzableException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                var storageResult = analyzeException(exception);
                if (!storageResult.IsOk())
                    return storageResult;
                throw;
            }
            return new StorageResult();
        }

        public async Task<StorageResult> HandleAnalyzableExceptionAsync(Func<Task> func)
        {
            try
            {
                await func();
            }
            catch (Exception exception)
            {
                var storageResult = analyzeException(exception);
                if (!storageResult.IsOk())
                    return storageResult;
                throw;
            }
            return new StorageResult();
        }


        public void HandleCommit(Action action)
        {
            using var transaction = dbContext.Database.BeginTransaction();
            action();
            transaction.Commit();
        }

        public async Task HandleCommitAsync(Func<Task> func)
        {
            using var transaction = await dbContext.Database.BeginTransactionAsync();
            await func();
            await transaction.CommitAsync();
        }

        public void HandleSave(Action<IBatch<TEntity>> action)
        {
            action(new Batch<TEntity>(dbContext, auditVisitor));
            dbContext.SaveChanges();
        }

        public async Task HandleSaveAsync(Func<IBatch<TEntity>, Task> action)
        {
            await action(new Batch<TEntity>(dbContext, auditVisitor));
            await dbContext.SaveChangesAsync();
        }
    }

    public class OrmStorage : IOrmStorage
    {
        private readonly DbContext context;
        private readonly Func<Exception, StorageResult> analyzeException;
        private readonly IAuditVisitor auditVisitor;

        public OrmStorage(
            DbContext context,
            Func<Exception, StorageResult> analyzeException,
            IAuditVisitor auditVisitor)
        {
            this.context = context;
            this.analyzeException = analyzeException;
            this.auditVisitor = auditVisitor;
        }

        public StorageResult Handle(Action<IBatch> action) 
        {
            var batch = new Batch(context, auditVisitor);
            try
            {
                action(batch);
                context.SaveChanges();
            }
            catch (Exception exception)
            {
                var storageResult = analyzeException(exception);
                if (!storageResult.IsOk())
                    return storageResult;
                throw;
            }
            return new StorageResult();
        }
    }
}