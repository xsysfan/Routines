﻿using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class StorageModelErrorTest
    {
        public StorageModelErrorTest()
        {
            TestIsland.Clear();
        }

        [TestMethod]
        public void TestDatabaseFieldRequiredError() 
        {
            var userContext = new UserContext("UnitTest");

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListLoggingTransients(logger);

            var routine = new AdminkaRoutineHandler(
                 ZoningSharedSourceProjectManager.GetConfiguration(),
                 ZoningSharedSourceProjectManager.GetConfigurationFactory(),
                 loggingTransientsFactory,
                 new MemberTag(this), userContext, new { input = "Input text" });
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new ParentRecord() { };
                    var storageResult = storage.Handle(batch => batch.Add(t0));
                    storageResult.Assert(1, "FieldCA", "ID or alternate id has no value", "Case ID absent");
                    // NOTE 1 : for ef core v1 - returns generic error (can't say which field is errored)
                    // NOTE 2 : id is incremented int (so there are no error that it was not setuped)
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new ParentRecord() { FieldA=null, FieldCA ="1", FieldCB1 = "2", FieldCB2 = "3" };
                    var storageError = storage.Handle(batch =>batch.Add(t0));
                    storageError.Assert(1, "FieldA", "Is required!", "Case isRequired string is null");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t0 = new ParentRecord() { FieldA = "100500", FieldCA = "1", FieldCB1 = "2", FieldCB2 = null };
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "FieldCB2", "ID or alternate id has no value", "Case alternate key (part of complex alternate key) absent");
                });
            });


            var parentRecord = new ParentRecord()
            {
                FieldA = "A",
                FieldB1 = "B",
                FieldB2 = "C",
                FieldCA = "1",
                FieldCB1 = "2",
                FieldCB2 = "3"
            };

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(parentRecord));
                    storageError.ThrowIfFailed("Add failed 1");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() { FieldA = "A", FieldB1 = "Ba", FieldB2 = "Ca",
                        FieldCA = "1a",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(1, "FieldA", "Allready used", "Case 3.");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord()
                    {
                        FieldA =  "Aa",
                        FieldB1 = "Ba",
                        FieldB2 = "Ca",
                        FieldCA =  "1a",
                        FieldCB1 = "2",
                        FieldCB2 = "3"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(2, new[] { "FieldCB1", "FieldCB2" }, null, "Case 4.");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() {
                        FieldA = "Aa",
                        FieldB1 = "B",
                        FieldB2 = "C",
                        FieldCA = "1a",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(2, new[] { "FieldB1", "FieldB2" }, null, "Case 5.");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ParentRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t2 = new ParentRecord() {
                        FieldA = "Aa",
                        FieldB1 = "Ba",
                        FieldB2 = "Ca",
                        FieldCA = "1",
                        FieldCB1 = "2a",
                        FieldCB2 = "3a"
                    };
                    var storageError = storage.Handle(batch => batch.Add(t2));
                    storageError.Assert(1, "FieldCA", null, "Case 6.");
                });
            });

            var typeRecord = new TypeRecord()
            {
                TestTypeRecordId="0000",
                TypeRecordName="TestType"
            };

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    storage.Handle(batch => batch.Add(typeRecord)).ThrowIfFailed("Can't add TestTypeRecord"); 
                });
            });

            var childRecord = new ChildRecord()
            {
                ParentRecordId = parentRecord.ParentRecordId,
                TypeRecordId = typeRecord.TestTypeRecordId,
                XmlField1 = "notxml",
                XmlField2 = "notxml"
            }; 

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<ChildRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    storage.Handle(batch => batch.Add(childRecord)).ThrowIfFailed("Can't add TestChildRecord");
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TypeRecord.TestTypeRecordId), null, "Case 7");
                });
            });

            // string that exceed its length limit
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0001x",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, "", null, "Case 8");
                });
            });

            // check constraint on INSERT
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0001",
                    TypeRecordName = "TestType2,,.."
                };
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 9");
                });
            });

            // check constraint on UPDATE
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TypeRecordName = "TestType2,,..";
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 10");
                });
            });

            // check NULL on UPDATE
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var t1 = repository.Find(e => e.TestTypeRecordId == "0000");
                    t1.TypeRecordName = null;
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    storageError.Assert(1, nameof(TypeRecord.TypeRecordName), null, "Case 11");
                });
            });
        }
    }
}