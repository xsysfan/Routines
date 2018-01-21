﻿using System;

using DashboardCode.Routines;
using DashboardCode.AdminkaV1.LoggingDom;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public class TraceService : ITraceService
    {
        public Trace GetTrace(Guid searchForCorrelationToken)
        {
            var routine = new WcfRoutine(new MemberTag(this), RoutineErrorDataContractConstants.FaultCodeNamespace, new { searchForCorrelationToken });
            return routine.HandleServicesContainer(traceService =>
            {
                return traceService.GetTrace(searchForCorrelationToken);
            });
        }
    }
}