﻿using BenchmarkDotNet.Attributes;
using System.Threading.Tasks;

namespace Benchmark
{
    [Config(typeof(CoreToolchain2JobConfig))]
    [RankColumn, MinColumn, MaxColumn, StdDevColumn, MedianColumn]
    [HtmlExporter, MarkdownExporter]
    [MemoryDiagnoser]
    public class BenchmarkAsyncNotAwaitInterface
    {
        string context = "text context";
        [Benchmark]
        public int CompletedAwait()
        {
            var t = new CompletedAwaitTest();
            var a = t.DoAsync(context);
            a.Wait();
            return t.Length;
        }

        [Benchmark]
        public int Completed()
        {
            var t = new CompletedTest();
            var a = t.DoAsync(context);
            a.Wait();
            return t.Length;
        }

        [Benchmark]
        public int Pragma()
        {
            var t = new PragmaAsyncTest();
            var a = t.DoAsync(context);
            a.Wait();
            return t.Length;
        }

        [Benchmark]
        public int FromResult()
        {
            var t = new FromResultTest();
            var t2 = t.DoAsync(context);
            return t2.Result;
        }

        [Benchmark]
        public int Yield()
        {
            var t = new YieldTest();
            var a = t.DoAsync(context);
            a.Wait();
            return t.Length;
        }

        public interface ITestInterface
        {
            int Length { get; }
            Task DoAsync(string context);
        }

        public interface ITestInterface2
        {
            Task<int> DoAsync(string context);
        }

        class CompletedAwaitTest : ITestInterface
        {
            public int Length { get; private set; }
            public async Task DoAsync(string context)
            {
                Length = context.Length;
                await Task.CompletedTask;
            }
        }

        class CompletedTest : ITestInterface
        {
            public int Length { get; private set; }
            public Task DoAsync(string context)
            {
                Length = context.Length;
                return Task.CompletedTask;
            }
        }

        class PragmaAsyncTest : ITestInterface
        {
            public int Length { get; private set; }
            #pragma warning disable 1998
            public async Task DoAsync(string context)
            {
                Length = context.Length;
                return;
            }
            #pragma warning restore 1998
        }

        class YieldTest : ITestInterface
        {
            public int Length { get; private set; }
            public async Task DoAsync(string context)
            {
                Length = context.Length;
                await Task.Yield();
            }
        }

        class FromResultTest : ITestInterface2
        {
            public async Task<int> DoAsync(string context)
            {
                var i = context.Length;
                return await Task.FromResult(i);
            }
        }
    }
}