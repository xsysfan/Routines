﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;

using DashboardCode.AdminkaV1.TestDom;

namespace DashboardCode.AdminkaV1.Injected.SqlServer.Test
{
    [TestClass]
    public class StorageConcurencyErrorTest
    {
        public StorageConcurencyErrorTest()
        {
            TestManager.Clear();
        }

        [TestMethod]
        public void TestConcurencyError()
        {
            var userContext = new UserContext("UnitTest");

            var logger = new List<string>();
            var loggingTransientsFactory = InjectedManager.ComposeListMemberLoggerFactory(logger);

            var routine = new AdminkaRoutineHandler(
                TestManager.ApplicationSettings.AdminkaStorageConfiguration,
                TestManager.ApplicationSettings.PerformanceCounters,
                TestManager.ApplicationSettings.ConfigurationContainerFactory,
                loggingTransientsFactory,
                new MemberTag(this), userContext, new { input = "Input text" });
            // check constraint on UPDATE
            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var t0 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType"
                };
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Add(t0));
                    storageError.ThrowIfFailed();
                });
            });

            routine.HandleOrmFactory((ormHandlerFactory) =>
            {
                var t1 = new TypeRecord()
                {
                    TestTypeRecordId = "0000",
                    TypeRecordName = "TestType2"
                };
                var repositoryHandler = ormHandlerFactory.Create<TypeRecord>();
                repositoryHandler.Handle((repository, storage) =>
                {
                    var storageError = storage.Handle(batch => batch.Modify(t1));
                    if (storageError.Count() != 1 || !storageError.ContainsLike("", "The record you are attempted to edit is currently being"))
                        throw new Exception("Test failed: not correct error. Case 1.");
                });
            });
        }
    }
}