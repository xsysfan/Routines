﻿using System;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.WcfService
{
    public class WcfRoutine : AdminkaRoutine
    {
        public WcfRoutine(RoutineTag routineTag, string faultCodeNamespace, object input) 
            : base(routineTag, GetUserContext(), TransformException(faultCodeNamespace), new ConfigurationNETFramework(), input)
        {
        }
        private static UserContext GetUserContext()
        {
            return new UserContext("Anonymous");
        }
        public static Func<Exception, RoutineTag, Func<Exception, string>, Exception> TransformException(string faultCodeNamespace)
        {
            return (ex,w,s)=>WcfException.TransformException(ex, w, faultCodeNamespace, s);
        }
    }
}