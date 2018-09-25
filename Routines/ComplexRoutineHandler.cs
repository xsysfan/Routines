﻿using System;
using System.Threading.Tasks;

namespace DashboardCode.Routines
{
    public class ComplexRoutineDisposeHandler<TClosure, TUserContext> :
        ComplexDisposeHandler<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ) : base (createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandler<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandler<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandler(
            Func<RoutineClosure<TUserContext>, TDerivedClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            ):base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandler<TClosure, TUserContext> : ComplexHandler<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext>
    {
        public ComplexRoutineHandler(
            Func<RoutineClosure<TUserContext>, TClosure> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ):base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerAsync<TClosure, TUserContext> :
        ComplexDisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext> where TClosure : IDisposable
    {
        public ComplexRoutineDisposeHandlerAsync(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }

    public class ComplexRoutineDisposeHandlerAsync<TClosure, TUserContext, TDerivedClosure> : ComplexDisposeHandlerAsync<TClosure, RoutineClosure<TUserContext>, TDerivedClosure>, IRoutineHandler<TClosure, TUserContext>
        where TDerivedClosure : IDisposable, TClosure
    {
        public ComplexRoutineDisposeHandlerAsync(
            Func<RoutineClosure<TUserContext>, Task<TDerivedClosure>> createResource,
            IHandler<RoutineClosure<TUserContext>> routineHandler
            ) : base(createResource, routineHandler)
        {
        }
    }

    public class ComplexRoutineHandlerAsync<TClosure, TUserContext> : ComplexHandlerAsync<TClosure, RoutineClosure<TUserContext>>, IRoutineHandler<TClosure, TUserContext>
    {
        public ComplexRoutineHandlerAsync(
            Func<RoutineClosure<TUserContext>, Task<TClosure>> createResource,
            IHandler<RoutineClosure<TUserContext>> handler
            ) : base(createResource, handler)
        {
        }
    }
}
