﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;

namespace DashboardCode.Routines.Storage.Ef6
{
    public class EntityMetaServiceContainer : IEntityMetaServiceContainer
    {
        readonly Dictionary<string, IOrmEntitySchemaAdapter> relationDbSchemaAdapters;
        readonly DbContext model;
        readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;
        

        public EntityMetaServiceContainer(
            DbContext model,
            Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze//,
            //Func<IMutableEntityType, IOrmEntitySchemaAdapter> ormEntitySchemaAdapterFactory,
            //Action<ModelBuilder> buildModel 
            )
        {
            this.model = model;
            this.analyze = analyze;

            this.relationDbSchemaAdapters = new Dictionary<string, IOrmEntitySchemaAdapter>();

            ////TODO:  constraints and unique indexes list should be integrated with configuration files or get from db directly

            //var conventionSet = new ConventionSet();
            //var modelBuilder = new ModelBuilder(conventionSet);
            //buildModel(modelBuilder);
            //mutableModel = modelBuilder.Model;

            //var entityTypes = mutableModel.GetEntityTypes();
            //foreach (var entityType in entityTypes)
            //{
            //    var lastIndexOfPoint = entityType.Name.LastIndexOf('.');
            //    var relationDbSchemaAdapter = ormEntitySchemaAdapterFactory(entityType); //new OrmEntitySchemaAdapter(entityType);
            //    relationDbSchemaAdapters.Add(entityType.Name, relationDbSchemaAdapter);
            //}
        }

        public IEntityMetaService<TEntity> Resolve<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);
            //var ormEntitySchemaAdapter = relationDbSchemaAdapters[type.FullName];

            var ormEntitySchemaAdapter2 = new OrmEntitySchemaAdapter<TEntity>(model, new SqlServerOrmEntitySchemaAdapter(model, type ));
            var entityStorageMetaService = new EntityStorageMetaService<TEntity>(ormEntitySchemaAdapter2, type, model, analyze);
            return entityStorageMetaService;
        }

        class EntityStorageMetaService<TEntity> : IEntityMetaService<TEntity> where TEntity : class
        {
            readonly OrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter;
            readonly Type type;
            readonly DbContext model;
            readonly Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze;
            public EntityStorageMetaService(
                OrmEntitySchemaAdapter<TEntity> ormEntitySchemaAdapter,
                Type type,
                DbContext model,
                Func<Exception, Type, IOrmEntitySchemaAdapter, string, StorageResult> analyze)
            {
                this.ormEntitySchemaAdapter = ormEntitySchemaAdapter;
                this.type = type;
                this.analyze = analyze;
                this.model = model;
            }

            public StorageResult Analyze(Exception ex)
            {
                var storageResult = analyze(ex, type, ormEntitySchemaAdapter, "");
                return storageResult;
            }

            public IOrmEntitySchemaAdapter<TEntity> GetOrmEntitySchemaAdapter()
            {
                var @output = new OrmEntitySchemaAdapter<TEntity>(model, ormEntitySchemaAdapter);
                return @output;
            }
        }
    }
}
