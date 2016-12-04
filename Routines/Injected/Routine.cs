﻿using System;
using System.Threading.Tasks;

namespace Vse.Routines.Injected
{
    public class Routine<TRoutineState>
    {
        private readonly IRoutineTransients<TRoutineState> routineTransients;
        private readonly IExceptionHandler exceptionHandler;
        private readonly IRoutineLogging routineLoggingFacade;
        private readonly object input;

        public Routine(IRoutineTransients<TRoutineState> routineTransients, object input)
        {
            this.routineTransients = routineTransients;
            this.input = input;
            routineLoggingFacade = routineTransients.ResolveRoutineLogging();
            exceptionHandler = routineTransients.ResolveExceptionHandler();
        }
        public void Handle(Action<TRoutineState> action)
        {
            exceptionHandler.Handle(
                () =>
                {
                    routineLoggingFacade.LogStart(input);
                    var context = routineTransients.ResolveStateService();
                    action(context);
                    routineLoggingFacade.LogFinish(true, null);
                },
                () => routineLoggingFacade.LogFinish(false, null)
            );
        }
        public TOutput Handle<TOutput>(Func<TRoutineState, TOutput> func)
        {
            var @value = default(TOutput);
            exceptionHandler.Handle(
                () =>
                {
                    routineLoggingFacade.LogStart(input);
                    var currentWorkflowContext = routineTransients.ResolveStateService();
                    @value = func(currentWorkflowContext);
                    routineLoggingFacade.LogFinish(true, @value);
                },
                ()=> routineLoggingFacade.LogFinish(false, null)
            );
            return @value;
        }
        public async Task<TOutput> HandleAsync<TOutput>(Func<TRoutineState, TOutput> func)
        {
            var @value = Task.Run(() =>
            {
                var output = default(TOutput);
                exceptionHandler.Handle(
                    () =>
                    {
                        routineLoggingFacade.LogStart(input);
                        var container = routineTransients.ResolveStateService();
                        output = func(container);
                        routineLoggingFacade.LogFinish(true, output);
                    },
                    () => routineLoggingFacade.LogFinish(false, null)
                );
                return output;
            });
            return await @value;
        }
    }
}
