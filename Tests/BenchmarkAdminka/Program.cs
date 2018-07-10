﻿using BenchmarkDotNet.Running;
using DashboardCode.AdminkaV1.Injected;

namespace BenchmarkAdminka
{
    class Program
    {
#if NETCOREAPP
        public readonly static ApplicationSettingsStandard ApplicationSettings = new ApplicationSettingsStandard();
#else
        public readonly static ApplicationSettingsClassic ApplicationSettings = new ApplicationSettingsClassic();
#endif
        static void Main(string[] args)
        {
            //var b = new BenchmarkAdminkaRoutineListLogger();
            //b.MeasureRoutineRepositoryErrorLogList();
            //var b = new BenchmarkAdminkaRoutineNLogLogger();
            //b.MeasureRoutineRepositoryErrorNLog();
            //b.MeasureRoutineRepositoryNLog();
            //b.MeasureRoutineRepositoryExceptionMailNLog();
            //b.MeasureRoutineNLog();
            //b.MeasureRoutineNoAuthorizationNLog();
            BenchmarkRunner.Run<BenchmarkAdminkaRoutineListLogger>();
            BenchmarkRunner.Run<BenchmarkAdminkaRoutineNLogLogger>();
        }
    }
}