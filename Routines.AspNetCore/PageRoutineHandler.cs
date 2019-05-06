﻿using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.Routines.AspNetCore
{
    public class PageRoutineHandler<TServiceContainer, TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, TUserContext, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;
        readonly Func<TUser, TUserContext> createUnitContext;

        public PageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<TUser, TUserContext> createUnitContext,
            Func<AspRoutineFeature, Func<object>, TUser, TUserContext, ContainerFactory, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.getContainerHandler = (user, userContext, containerFactory) => getContainerHandler(aspRoutineFeature, getInput, user, userContext, containerFactory);
            this.createUnitContext = createUnitContext;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(user, userContext, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure));
            return actionResult;
        }

        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = getUserAndFailedActionResultInitialisedAsync().Result;
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(user, userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(user, userContext, containerFactory);
            var actionResult = await handler.HandleAsync((container, closure) => func(container, closure, user));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(user, userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, TUser, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getContainerHandler(user, userContext, containerFactory);
            var actionResult = handler.Handle((container, closure) => func(container, closure, user));
            return actionResult;
        }
    }

    public class PageRoutineHandler<TUserContext, TUser>
    {
        readonly Func<Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync;
        readonly Func<TUser, TUserContext> createUnitContext;
        readonly Func<TUser, TUserContext, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler;

        public PageRoutineHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Task<(IActionResult forbiddenActionResult, TUser user, ContainerFactory containerFactory)>> getUserAndFailedActionResultInitialisedAsync,
            Func<TUser, TUserContext> createUserContext,
            Func<AspRoutineFeature, Func<object>, TUser,  TUserContext, ContainerFactory, IRoutineHandler<TUser, TUserContext>> getUserHandler
            )
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            Func<object> getInput = () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request);

            this.getUserAndFailedActionResultInitialisedAsync = () => getUserAndFailedActionResultInitialisedAsync(aspRoutineFeature);
            this.createUnitContext = createUserContext;
            this.getUserHandler = (user, userContext, containerFactory) => getUserHandler(aspRoutineFeature, getInput, user, userContext,  containerFactory);
        }

        #region HandleUserAsync
        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, IActionResult> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getUserHandler(user, userContext, containerFactory);
            var actionResult = handler.Handle((u, closure) => func(u, closure));
            return actionResult;
        }

        public async Task<IActionResult> HandleAsync(Func<TUser, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            var (forbiddenActionResult, user, containerFactory) = await getUserAndFailedActionResultInitialisedAsync();
            if (forbiddenActionResult != null)
                return forbiddenActionResult;
            var userContext = createUnitContext(user);
            var handler = getUserHandler(user, userContext, containerFactory);
            var actionResult = await handler.HandleAsync((u, closure) => func(u, closure));
            return actionResult;
        }
        #endregion
    }

    public class PageRoutineAnonymousHandler<TServiceContainer, TUserContext>
    {
        readonly Func<ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler;

        public PageRoutineAnonymousHandler(
            PageModel pageModel,
            Func<AspRoutineFeature, Func<object>, ComplexRoutineHandler<TServiceContainer, TUserContext>> getContainerHandler)
        {
            var aspRoutineFeature = AspNetCoreManager.GetAspRoutineFeature(pageModel);
            this.getContainerHandler = () => getContainerHandler(
                aspRoutineFeature,
                () => AspNetCoreManager.GetRequest(pageModel.HttpContext.Request));
        }

        public Task<IActionResult> HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task<IActionResult>> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }

        public IActionResult Handle(Func<TServiceContainer, RoutineClosure<TUserContext>, IActionResult> func)
        {
            return getContainerHandler()
                .Handle((container, closure) => func(container, closure));
        }

        public Task HandleAsync(Func<TServiceContainer, RoutineClosure<TUserContext>, Task> func)
        {
            return getContainerHandler()
                .HandleAsync((container, closure) => func(container, closure));
        }
    }
}