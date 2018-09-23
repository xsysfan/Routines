﻿using System;

using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.Routines;

namespace DashboardCode.AdminkaV1.DataAccessEfCore.Services
{
    public class TraceService : ITraceService
    {
        readonly RoutineDisposeHandler<AdminkaDbContext, UserContext> dbContextHandler;
        public TraceService(RoutineDisposeHandler<AdminkaDbContext, UserContext> dbContextHandler)
            => this.dbContextHandler = dbContextHandler;

        public Trace GetTrace(Guid correlationToken)
        {
            return dbContextHandler.Handle<Trace>(
                (dbContext,closure) =>
                    throw new AdminkaDataAccessEfCoreException($"User exception from '{nameof(GetTrace)}'", closure,  "TEST")
                );
        }

        public void ResetTrace(Guid correlationToken)
        {
            dbContextHandler.Handle(
                (dbContext, closure) =>
                    throw new AdminkaDataAccessEfCoreException($"User exception from '{nameof(ResetTrace)}'", closure, "TEST")
                );
        }
    }
}