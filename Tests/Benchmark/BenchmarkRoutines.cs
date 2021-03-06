﻿using BenchmarkDotNet.Attributes;
using DashboardCode.Routines;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkRoutines
    {
        [Benchmark]
        public void IncludesCopy()
        {

            var source = TestTools.CreateTestModel();
            var destination = new TestModel();
            var includes = TestTools.CreateInclude();
            ObjectExtensions.Copy(source, destination, includes);
        }
        [Benchmark]
        public void IncludesEquals()
        {
            var source = TestTools.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques)
                            .ThenInclude(i => i.IndexName) // compare
                    .Include(i => i.ListTest);
            var b2 = ObjectExtensions.Equals(source, source, includes);
        }

        [Benchmark]
        public void IncludesClone()
        {
            var source = TestTools.CreateTestModel();
            Include<TestModel> includes
                = includable => includable
                    .IncludeAll(i => i.TestChilds)
                        .ThenIncludeAll(i => i.Uniques);
            var destination = ObjectExtensions.Clone(source, includes);
        }

        [Benchmark]
        public void Increment()
        {
            var i = 0; i++;
        }

        [Benchmark]
        public void Concantent()
        {
            var a = "asdasd1";
            var b = "asdasd2";
            var c = a + b;
        }
    }
}
