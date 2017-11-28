﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using DashboardCode.Routines.Storage.EfCore.Relational;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.Routines.Storage.EfModelTest.EfCore
{
    public class DbContextTests
    {
        public static void Reset(string connectionString)
        {
            using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString)))
            {
                TestService.Clear(new AdoBatch(dbContext));
                TestService.Reset(StorageFactory.CreateStorage(dbContext));
            }
        }

        public static int[] ParallelTestImpl(int parallelTaskCount, string connectionString)
        {
            List<List<string>> buffers = new List<List<string>>();

            var seria = Enumerable.Range(1, parallelTaskCount).ToList();
            Parallel.ForEach(seria, t =>
            {
                var messages = new List<string>();
                buffers.Add(messages);
                Action<string> verbose = (text) => {
                    messages.Add(text);
                    Console.WriteLine(t+") "+text);
                    Console.WriteLine();
                };

                using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString), verbose))
                {
                    var parentRecords = dbContext.ParentRecords
                       .Include(e => e.ParentRecordHierarchyRecordMap)
                       .ThenInclude(e => e.HierarchyRecord).ToList();
                    var parentRecord = parentRecords.
                       First(e => e.FieldA == "1_A");
                    //StraightEfTests.TestHierarchy(dbContext);
                }
            });

            return buffers.Select(b => b.Count).ToArray();
        }

        public static void TestHierarchy(MyDbContext dbContext)
        {
                var parentRecord = dbContext.ParentRecords
                    .Include(e => e.ParentRecordHierarchyRecordMap)
                    .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");
                var count1 = parentRecord.ParentRecordHierarchyRecordMap.Count(); // 5
                var only2 = parentRecord.ParentRecordHierarchyRecordMap.Take(2).ToList();
                var count2 = only2.Count(); // 2
                foreach (var map in only2)
                {
                    dbContext.Entry(map).State = EntityState.Detached;
                    //parentRecord.ParentRecordHierarchyRecordMap.Remove(map);
                }

                EntityEntry<ParentRecord> entry = dbContext.Entry(parentRecord);
                var col = entry.Collection(e => e.ParentRecordHierarchyRecordMap);
                col.Load();
                var oldRelations = parentRecord.ParentRecordHierarchyRecordMap;
                var tmp = new List<ParentRecordHierarchyRecord>();
                foreach (var e1 in oldRelations)
                    if (!only2.Any(e2 => e1.HierarchyRecordId == e2.HierarchyRecordId))
                        tmp.Add(e1);
                foreach (var e in tmp)
                    oldRelations.Remove(e);

                var count2b = parentRecord.ParentRecordHierarchyRecordMap.Count();

                //foreach (var e1 in only2)
                //    if (!oldRelations.Any(e2 => e1.HierarchyRecordId == e2.HierarchyRecordId))
                //        oldRelations.Add(e1);

                dbContext.SaveChanges();

                var parentRecord2 = dbContext.ParentRecords
                    .Include(e => e.ParentRecordHierarchyRecordMap)
                    .ThenInclude(e => e.HierarchyRecord).First(e => e.FieldA == "1_A");

                var count3 = parentRecord2.ParentRecordHierarchyRecordMap.Count();

                if (count3 != 2)
                {
                    var str = "this is an error from ef6 point of view";
                }
        }

        internal static void InMemoryTest()
        {
            var connectionString = Guid.NewGuid().ToString();
            using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString, true)))
                TestService.Reset(StorageFactory.CreateStorage(dbContext));

            Do(connectionString, true);
        }

        internal static void SqlServerTest(string connectionString)
        {
            using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString)))
            {
                TestService.Clear(new AdoBatch(dbContext));
                TestService.Reset(StorageFactory.CreateStorage(dbContext));
            }

            Do(connectionString, false);
        }

        private static void Do(string connectionString, bool inMemory)
        {
            var messages = new List<string>();
            Action<string> verbose = (text) => {
                messages.Add(text);
                Console.WriteLine(text);
                Console.WriteLine();
            };

            Console.WriteLine("Check connection string:");
            Console.WriteLine(connectionString);

            int id = 0;
            // TODO: github error - verbose ruins test
            using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString, inMemory), inMemory ? null : verbose))
            {
                var parentRecords = dbContext.ParentRecords
                   .Include(e => e.ParentRecordHierarchyRecordMap)
                   .ThenInclude(e => e.HierarchyRecord).ToList();
                var parentRecord = parentRecords.First(e => e.FieldA == "1_A");
                id = parentRecord.ParentRecordId;

                DbContextTests.TestHierarchy(dbContext);
            }

            //using (var dbContext = new MyDbContext(MyDbContext.BuildOptionsBuilder(connectionString, inMemory), verbose))
            //{
            //    var storage = new OrmStorage<ParentRecord>(dbContext,
            //        (ex) => ExceptionExtensions.Analyze(ex, null),
            //        (o) => {; });
            //    var parentRecord = new ParentRecord { ParentRecordId = id };

            //    storage.Handle((b) =>
            //    {
            //        b.Modify(parentRecord);
            //    });
            //}
        }

        internal static void ParallelTest(string connectionString)
        {
            DbContextTests.Reset(connectionString);
            int parallelTaskCount = 32;
            var c1 = DbContextTests.ParallelTestImpl(parallelTaskCount, connectionString);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var c2 = DbContextTests.ParallelTestImpl(parallelTaskCount, connectionString);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            var instancesInPoolCount = StatefullLoggerFactoryPool.Instance.Count();
            if (c1.Length != c2.Length || c1.Length != parallelTaskCount)
                throw new Exception("Some log messages are not in correct buffers");
            int[] union = c1.Concat(c2).ToArray();
            var distinctLengths = union.Distinct().ToList();
            if (distinctLengths.Count != 2)
                throw new Exception("Something unexpected 1. There should be two types of buffers: with new and with pooled StatefullLoggerFactories");
            var k0 = distinctLengths[0];
            var k1 = distinctLengths[1];

            var countK0 = union.Where(e => e == k0).Count();
            var countK1 = union.Where(e => e == k1).Count();
            if (!(countK0 == instancesInPoolCount || countK1 == instancesInPoolCount
                || countK1 - 1 == instancesInPoolCount
                || countK1 - 1 == instancesInPoolCount))
                throw new Exception("Something unexpected 2. cound of dbcontext with pooled messages should be equal to number of StatefullLoggerFactories instances (one can created by other tests) ");
        }
    }
}