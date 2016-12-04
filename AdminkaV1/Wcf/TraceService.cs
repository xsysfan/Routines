﻿using System;
using Vse.AdminkaV1.DomLogging;
using Vse.AdminkaV1.Injected;
using Vse.Routines;
using DataContactConstants = Vse.AdminkaV1.Wcf.Contracts.RoutineErrorDataContractConstants;
using IService = Vse.AdminkaV1.Wcf.Contracts.ITraceService;

namespace Vse.AdminkaV1.Wcf
{
    public class TraceService : IService
    {
        public Trace GetTrace(Guid correlationToken)
        {
            var input = new { correlationToken = correlationToken };
            var routine = new WcfRoutine(new RoutineTag(this), DataContactConstants.FaultCode, input);
            return routine.Handle((container, dataAccess) =>
            {
                var servicesContainer = new ServicesContainer(dataAccess);
                var service = servicesContainer.ResolveTraceService();
                return service.GetTrace(correlationToken);
            });
        }
    }
}