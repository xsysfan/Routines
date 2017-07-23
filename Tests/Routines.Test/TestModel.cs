﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Test
{
    public class TestChild
    {
        public List<Unique> Uniques { get; set; }
    }
    public class Item
    {
        public string F1 { get; set; }
        public string F2 { get; set; }

        public List<Item> Items { get; set; }
    }
    public class TestModel
    {
        public StorageModel StorageModel { get; set; }
        public int[] Test { get; set; }
        public IEnumerable<Guid> ListTest { get; set; }
        public IEnumerable<TestChild> TestChilds { get; set; }
        public ICollection<CultureInfo> CultureInfos { get; set; }

        public string PropertyText { get; set; }
        public int PropertyInt { get; set; }

        private string Name2 { get; set; }

        public string this[int index]
        {
            get
            {
                return Name2[index].ToString();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public MessageStruct Message { get; set; }
        public struct MessageStruct
        {
            public string TextMsg { get; set; }
            public DateTime DateTimeMsg { get; set; }
            public int? IntNullableMsg { get; set; }
        }

        public int? IntNullable1 { get; set; }
        public int? IntNullable2 { get; set; }
    }

    public static class TestTool
    {
        public static Include<TestModel> CreateInclude()
        {
            Include<TestModel> includes
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Name)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                            .ThenInclude(i => i.Namespace)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenInclude(i => i.Attributes)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.TableName)
                    .Include(i => i.Test)
                    .Include(i => i.ListTest)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                             .ThenInclude(i => i.IndexName)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                             .ThenIncludeAll(i => i.Fields)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.TextMsg)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.DateTimeMsg)
                    .Include(i => i.Message)
                         .ThenInclude(i => i.IntNullableMsg)
                    .Include(i => i.IntNullable1)
                    .Include(i => i.IntNullable2);
            return includes;
        }

        public static Include<TestModel> CreateIncludeWithoutLeafs()
        {
            Include<TestModel> includes
                = includable => includable
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Entity)
                    .Include(i => i.StorageModel)
                        .ThenInclude(i => i.Key)
                            .ThenIncludeAll(i => i.Attributes)
                    .IncludeAll(i => i.Test)
                    .IncludeAll(i => i.ListTest)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                    .Include(i => i.StorageModel)
                         .ThenIncludeAll(i => i.Uniques)
                             .ThenIncludeAll(i => i.Fields)
                    .Include(i => i.Message);
            return includes;
        }

        public static TestModel CreateTestModel()
        {
            var source = new TestModel()
            {
                StorageModel = new StorageModel()
                {
                    TableName = "TableName1",
                    Entity = new Entity() { Name = "EntityName1", Namespace = "EntityNamespace1" },
                    Key = new Key() { Attributes = new[] { "FieldA1", "FieldA2" } },
                    Uniques = new[] { new Unique { Fields = new[] { "FieldU1" }, IndexName = "IndexName1" }, new Unique { Fields = new[] { "FieldU2" }, IndexName = "IndexName2" } }

                },
                Test = new[] { 1, 2, 3 },
                ListTest = new List<Guid>() { Guid.Parse("360bc50a-4d9f-4703-bbea-58f67a6ff475"), Guid.Parse("f2ecf4d8-f4a6-446c-a363-cc79b02decdd") },
                CultureInfos = new List<CultureInfo>() { CultureInfo.CurrentCulture, CultureInfo.InvariantCulture },
                PropertyText = "sampleTest",
                PropertyInt = 1234
            };
            source.TestChilds = new HashSet<TestChild>() { new TestChild { Uniques = source.StorageModel.Uniques.ToList() } };
            source.Message    = new TestModel.MessageStruct() { TextMsg = "Initial", DateTimeMsg = DateTime.MaxValue, IntNullableMsg = 7 };
            source.IntNullable2 = 555;
            return source;
        }

        public static TestModel CreateTestModelWithNulls()
        {
            var source = new TestModel()
            {
                StorageModel= new StorageModel(),
                //StorageModel = new StorageModel()
                //{
                //    TableName = "TableName1",
                //    Entity = new Entity() { Name = "EntityName1", Namespace = null },
                //    Key = new Key() { Attributes = new[] { "FieldA1", null, "FieldA2" } },
                //    Uniques = new[] { new Unique { Fields = new[] { "FieldU1" }, IndexName = "IndexName1" }, new Unique { Fields = new[] { "FieldU2" }, IndexName = "IndexName2" } }

                //},
                Test = new[] { 1, 2, 3 },
                ListTest = new List<Guid>() {  Guid.NewGuid(), Guid.NewGuid() },
                CultureInfos = new List<CultureInfo>() { CultureInfo.CurrentCulture, CultureInfo.InvariantCulture },
                PropertyText = "sampleTest",
                PropertyInt = 1234
            };
            //source.TestChilds = new HashSet<TestChild>() { new TestChild { Uniques = source.StorageModel.Uniques.ToList() } };
            source.Message = new TestModel.MessageStruct() { TextMsg = "Initial", DateTimeMsg = DateTime.Now, IntNullableMsg = null };
            source.IntNullable2 = null;
            return source;
        }
    }
}
