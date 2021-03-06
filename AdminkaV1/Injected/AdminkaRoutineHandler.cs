﻿using System;
using System.Runtime.CompilerServices;

using DashboardCode.Routines;
using DashboardCode.Routines.Logging;
using DashboardCode.Routines.Configuration;

using DashboardCode.AdminkaV1.Injected.Logging;
using DashboardCode.AdminkaV1.Injected.Telemetry;
using System.Threading.Tasks;

namespace DashboardCode.AdminkaV1.Injected
{
    public class AdminkaAnonymousRoutineHandler : AdminkaRoutineHandlerBase<AnonymousUserContext>
    {
        public AdminkaAnonymousRoutineHandler(
            ApplicationSettings applicationSettings,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            string auditStamp,
            object input) : this(
                  applicationSettings,
                  InjectedManager.ComposeNLogMemberLoggerFactory(null),
                  hasVerboseLoggingPrivilege,
                  memberTag,
                  auditStamp,
                  input
                )
        {

        }

        public AdminkaAnonymousRoutineHandler( 
            ApplicationSettings applicationSettings,
            Func<Guid, MemberTag, IMemberLogger> loggingTransientsFactory,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            string auditStamp,
            object input ):base(
                  applicationSettings,
                  (u)=> auditStamp,
                  new AdminkaRoutineHandlerFactory<AnonymousUserContext>(
                    correlationToken: Guid.NewGuid(),
                    InjectedManager.DefaultRoutineTagTransformException,
                    loggingTransientsFactory,
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, auditStamp),
                            new AnonymousUserContext(auditStamp),
                            hasVerboseLoggingPrivilege: hasVerboseLoggingPrivilege,
                            input)
                )
        {

        }

        public AdminkaAnonymousRoutineHandler(
            ApplicationSettings applicationSettings,
            string auditStamp,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : this(
                applicationSettings,
                new AnonymousUserContext(auditStamp),
                input,
                correlationToken,
                documentBuilder,
                controllerNamespace,
                controllerName,
                member
            )
        {
        }

        public AdminkaAnonymousRoutineHandler(
            ApplicationSettings applicationSettings,
            AnonymousUserContext anonymousUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                anonymousUserContext,
                getAuditStamp: u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: anonymousUserContext.AuditStamp,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
        public AdminkaAnonymousRoutineHandler(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Exception, Guid, MemberTag, /*Func<Exception, string>,*/ Exception> transformException,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            MemberTag memberTag,
            AnonymousUserContext anonymousUserContext,
            object input
            ) : base(
                applicationSettings,
                performanceCounters,
                configurationContainerFactory,
                transformException,
                correlationToken,
                documentBuilder,
                memberTag,
                anonymousUserContext,
                false, //getVerboseLoggingFlag: (userContext) => "VerboseLogging", // (userContext) => (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null
                configurationFor: anonymousUserContext.AuditStamp,
                u => u.AuditStamp,
                input
            )
        {
        }
    }

    public class AdminkaInternalUserRoutineHandler : AdminkaRoutineHandlerBase<UserContext> 
    {
        public AdminkaInternalUserRoutineHandler(
            ApplicationSettings applicationSettings,
            UserContext internalUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                internalUserContext,
                u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: internalUserContext.User.LoginName,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
    }

    public class AdminkaRoutineHandlerBase<TUserContext> : ComplexRoutineHandler<PerCallContainer<TUserContext>, TUserContext>
    {
        #region constructors with usercontext
        public AdminkaRoutineHandlerBase(
                ApplicationSettings applicationSettings,
                TUserContext userContext,
                Func<TUserContext, string> getAuditStamp,
                object input,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                MemberTag memberTag
            ) : this( // to final
                applicationSettings,
                getAuditStamp,
                new AdminkaRoutineHandlerFactory<TUserContext>(
                    correlationToken,
                    InjectedManager.DefaultRoutineTagTransformException,
                    InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, configurationFor),
                            userContext,
                            hasVerboseLoggingPrivilege,
                            input)
                )
        {
        }

        // used by wcf and test (predefined UserContext with its own transformException)
        public AdminkaRoutineHandlerBase(
                ApplicationSettings applicationSettings,
                IPerformanceCounters performanceCounters,
                IConfigurationContainerFactory configurationFactory,
                Func<Exception, Guid, MemberTag,/* Func<Exception, string>,*/ Exception> routineTransformException,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                MemberTag memberTag,
                TUserContext userContext,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                Func<TUserContext, string> getAuditStamp,
                object input
            ) : this(
                applicationSettings,
                performanceCounters,
                routineTransformException,
                InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                InjectedManager.CreateContainerFactory(configurationFactory).CreateContainer(memberTag, configurationFor),
                correlationToken,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }

        // Used as "pre-final-2" stage to prepare containerFactory, memberTag and usercontext
        private AdminkaRoutineHandlerBase(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            Func<Exception, Guid, MemberTag, /*Func<Exception, string>, */Exception> routineTransformException,
            Func<Guid, MemberTag, IMemberLogger> composeLoggers,
            IContainer container,
            Guid correaltionToken,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input
        ) : this(
                applicationSettings,
                new AdminkaRoutineHandlerFactory<TUserContext>(correaltionToken, 
                    routineTransformException, 
                    composeLoggers, 
                    performanceCounters),
                container,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }
        #endregion

        // Used as "pre-final-1"  stage to prepare container
        private AdminkaRoutineHandlerBase(
            ApplicationSettings applicationSettings,
            AdminkaRoutineHandlerFactory<TUserContext> routineLogger,
            IContainer container,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input) :
            this(
                applicationSettings,
                getAuditStamp,
                routineLogger.CreateLoggingHandler(memberTag, container, userContext,
                    hasVerboseLoggingPrivilege,
                    input)
                )
        { 
        }

        // final
        public AdminkaRoutineHandlerBase(
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> getAuditStamp,
            IHandler<RoutineClosure<TUserContext>> routineHandler
        ) : base(
                closure => new PerCallContainer<TUserContext>(closure, applicationSettings, getAuditStamp),
                routineHandler
            )
        {
        }

    }

    // -----------
    public class AdminkaAnonymousRoutineHandlerAsync : AdminkaRoutineHandlerBaseAsync<AnonymousUserContext>
    { 
           public AdminkaAnonymousRoutineHandlerAsync(
            ApplicationSettings applicationSettings,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            string auditStamp,
            object input) : this(
                  applicationSettings,
                  InjectedManager.ComposeNLogMemberLoggerFactory(null),
                  hasVerboseLoggingPrivilege,
                  memberTag,
                  auditStamp,
                  input
                )
        {

        }

        public AdminkaAnonymousRoutineHandlerAsync( 
            ApplicationSettings applicationSettings,
            Func<Guid, MemberTag, IMemberLogger> loggingTransientsFactory,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            string auditStamp,
            object input ):base(
                  applicationSettings,
                  (u)=> auditStamp,
                  new AdminkaRoutineHandlerFactory<AnonymousUserContext>(
                    correlationToken: Guid.NewGuid(),
                    InjectedManager.DefaultRoutineTagTransformException,
                    loggingTransientsFactory,
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, auditStamp),
                            new AnonymousUserContext(auditStamp),
                            hasVerboseLoggingPrivilege: hasVerboseLoggingPrivilege,
                            input)
                )
        {

        }

        public AdminkaAnonymousRoutineHandlerAsync(
            ApplicationSettings applicationSettings,
            string auditStamp,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : this(
                applicationSettings,
                new AnonymousUserContext(auditStamp),
                input,
                correlationToken,
                documentBuilder,
                controllerNamespace,
                controllerName,
                member
            )
        {
        }

        public AdminkaAnonymousRoutineHandlerAsync(
            ApplicationSettings applicationSettings,
            AnonymousUserContext anonymousUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                anonymousUserContext,
                getAuditStamp: u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: anonymousUserContext.AuditStamp,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
        public AdminkaAnonymousRoutineHandlerAsync(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Exception, Guid, MemberTag, /*Func<Exception, string>,*/ Exception> transformException,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            MemberTag memberTag,
            AnonymousUserContext anonymousUserContext,
            object input
            ) : base(
                applicationSettings,
                performanceCounters,
                configurationContainerFactory,
                transformException,
                correlationToken,
                documentBuilder,
                memberTag,
                anonymousUserContext,
                false, //getVerboseLoggingFlag: (userContext) => "VerboseLogging", // (userContext) => (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null
                configurationFor: anonymousUserContext.AuditStamp,
                u => u.AuditStamp,
                input
            )
        {
        }
    }

    public class AdminkaInternalUserRoutineHandlerAsync : AdminkaRoutineHandlerBaseAsync<UserContext> 
    {
        public AdminkaInternalUserRoutineHandlerAsync(
            ApplicationSettings applicationSettings,
            UserContext internalUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                internalUserContext,
                u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: internalUserContext.User.LoginName,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
    }

    public class AdminkaRoutineHandlerBaseAsync<TUserContext> : ComplexRoutineHandlerAsync<PerCallContainer<TUserContext>, TUserContext>
    {
        #region constructors with usercontext
        public AdminkaRoutineHandlerBaseAsync(
                ApplicationSettings applicationSettings,
                TUserContext userContext,
                Func<TUserContext, string> getAuditStamp,
                object input,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                MemberTag memberTag
            ) : this( // to final
                applicationSettings,
                getAuditStamp,
                new AdminkaRoutineHandlerFactory<TUserContext>(
                    correlationToken,
                    InjectedManager.DefaultRoutineTagTransformException,
                    InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, configurationFor),
                            userContext,
                            hasVerboseLoggingPrivilege,
                            input)
                )
        {
        }

        // used by wcf and test (predefined UserContext with its own transformException)
        public AdminkaRoutineHandlerBaseAsync(
                ApplicationSettings applicationSettings,
                IPerformanceCounters performanceCounters,
                IConfigurationContainerFactory configurationFactory,
                Func<Exception, Guid, MemberTag,/* Func<Exception, string>,*/ Exception> routineTransformException,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                MemberTag memberTag,
                TUserContext userContext,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                Func<TUserContext, string> getAuditStamp,
                object input
            ) : this(
                applicationSettings,
                performanceCounters,
                routineTransformException,
                InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                InjectedManager.CreateContainerFactory(configurationFactory).CreateContainer(memberTag, configurationFor),
                correlationToken,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }

        // Used as "pre-final-2" stage to prepare containerFactory, memberTag and usercontext
        private AdminkaRoutineHandlerBaseAsync(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            Func<Exception, Guid, MemberTag, /*Func<Exception, string>, */Exception> routineTransformException,
            Func<Guid, MemberTag, IMemberLogger> composeLoggers,
            IContainer container,
            Guid correaltionToken,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input
        ) : this(
                applicationSettings,
                new AdminkaRoutineHandlerFactory<TUserContext>(correaltionToken,
                    routineTransformException,
                    composeLoggers,
                    performanceCounters),
                container,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }
        #endregion

        // Used as "pre-final-1"  stage to prepare container
        private AdminkaRoutineHandlerBaseAsync(
            ApplicationSettings applicationSettings,
            AdminkaRoutineHandlerFactory<TUserContext> routineLogger,
            IContainer container,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input) :
            this(
                applicationSettings,
                getAuditStamp,
                routineLogger.CreateLoggingHandler(memberTag, container, userContext,
                    hasVerboseLoggingPrivilege,
                    input)
                )
        {
        }

        // final
        public AdminkaRoutineHandlerBaseAsync(
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> getAuditStamp,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler
        ) : base(
                closure => new PerCallContainer<TUserContext>(closure, applicationSettings, getAuditStamp),
                routineHandler
            )
        {
        }
    }

    // ------------
    /*
    public class AdminkaAnonymousRoutineHandlerAsync2 : AdminkaRoutineHandlerBaseAsync2<AnonymousUserContext>
    {
        public AdminkaAnonymousRoutineHandlerAsync2(
         ApplicationSettings applicationSettings,
         bool hasVerboseLoggingPrivilege,
         MemberTag memberTag,
         string auditStamp,
         object input) : this(
               applicationSettings,
               InjectedManager.ComposeNLogMemberLoggerFactory(null),
               hasVerboseLoggingPrivilege,
               memberTag,
               auditStamp,
               input
             )
        {

        }

        public AdminkaAnonymousRoutineHandlerAsync2(
            ApplicationSettings applicationSettings,
            Func<Guid, MemberTag, IMemberLogger> loggingTransientsFactory,
            bool hasVerboseLoggingPrivilege,
            MemberTag memberTag,
            string auditStamp,
            object input) : base(
                  applicationSettings,
                  (u) => auditStamp,
                  new AdminkaRoutineHandlerFactory<AnonymousUserContext>(
                    correlationToken: Guid.NewGuid(),
                    InjectedManager.DefaultRoutineTagTransformException,
                    loggingTransientsFactory,
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, auditStamp),
                            new AnonymousUserContext(auditStamp),
                            hasVerboseLoggingPrivilege: hasVerboseLoggingPrivilege,
                            input)
                )
        {

        }

        public AdminkaAnonymousRoutineHandlerAsync2(
            ApplicationSettings applicationSettings,
            string auditStamp,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : this(
                applicationSettings,
                new AnonymousUserContext(auditStamp),
                input,
                correlationToken,
                documentBuilder,
                controllerNamespace,
                controllerName,
                member
            )
        {
        }

        public AdminkaAnonymousRoutineHandlerAsync2(
            ApplicationSettings applicationSettings,
            AnonymousUserContext anonymousUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                anonymousUserContext,
                getAuditStamp: u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: anonymousUserContext.AuditStamp,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
        public AdminkaAnonymousRoutineHandlerAsync2(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            IConfigurationContainerFactory configurationContainerFactory,
            Func<Exception, Guid, MemberTag,  Exception> transformException,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            MemberTag memberTag,
            AnonymousUserContext anonymousUserContext,
            object input
            ) : base(
                applicationSettings,
                performanceCounters,
                configurationContainerFactory,
                transformException,
                correlationToken,
                documentBuilder,
                memberTag,
                anonymousUserContext,
                false, //getVerboseLoggingFlag: (userContext) => "VerboseLogging", // (userContext) => (userContext?.User?.HasPrivilege(Privilege.VerboseLogging) ?? false) ? Privilege.VerboseLogging : null
                configurationFor: anonymousUserContext.AuditStamp,
                u => u.AuditStamp,
                input
            )
        {
        }
    }

    public class AdminkaInternalUserRoutineHandlerAsync2 : AdminkaRoutineHandlerBaseAsync2<UserContext>
    {
        public AdminkaInternalUserRoutineHandlerAsync2(
            ApplicationSettings applicationSettings,
            UserContext internalUserContext,
            object input,
            Guid correlationToken,
            ITraceDocumentBuilder documentBuilder,
            string controllerNamespace,
            string controllerName,
            [CallerMemberName] string member = null
            ) : base(
                applicationSettings,
                internalUserContext,
                u => u.AuditStamp,
                input,
                correlationToken: correlationToken,
                documentBuilder: documentBuilder,
                hasVerboseLoggingPrivilege: false,
                configurationFor: internalUserContext.User.LoginName,
                memberTag: new MemberTag(controllerNamespace, controllerName, member)
            )
        {
        }
    }
*/
    /*
    public class AdminkaRoutineHandlerBaseAsync2<TUserContext> : ComplexRoutineHandlerAsync2<PerCallContainer<TUserContext>, TUserContext>
    {
        #region constructors with usercontext
        public AdminkaRoutineHandlerBaseAsync2(
                ApplicationSettings applicationSettings,
                TUserContext userContext,
                Func<TUserContext, string> getAuditStamp,
                object input,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                MemberTag memberTag
            ) : this( // to final
                applicationSettings,
                getAuditStamp,
                new AdminkaRoutineHandlerFactory<TUserContext>(
                    correlationToken,
                    InjectedManager.DefaultRoutineTagTransformException,
                    InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                    applicationSettings.PerformanceCounters)
                        .CreateLoggingHandler(
                            memberTag,
                            InjectedManager.CreateContainerFactory(
                                applicationSettings.ConfigurationContainerFactory
                                ).CreateContainer(memberTag, configurationFor),
                            userContext,
                            hasVerboseLoggingPrivilege,
                            input)
                )
        {
        }

        // used by wcf and test (predefined UserContext with its own transformException)
        public AdminkaRoutineHandlerBaseAsync2(
                ApplicationSettings applicationSettings,
                IPerformanceCounters performanceCounters,
                IConfigurationContainerFactory configurationFactory,
                Func<Exception, Guid, MemberTag, Exception> routineTransformException,
                Guid correlationToken,
                ITraceDocumentBuilder documentBuilder,
                MemberTag memberTag,
                TUserContext userContext,
                bool hasVerboseLoggingPrivilege,
                string configurationFor,
                Func<TUserContext, string> getAuditStamp,
                object input
            ) : this(
                applicationSettings,
                performanceCounters,
                routineTransformException,
                InjectedManager.ComposeNLogMemberLoggerFactory(documentBuilder),
                InjectedManager.CreateContainerFactory(configurationFactory).CreateContainer(memberTag, configurationFor),
                correlationToken,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }

        // Used as "pre-final-2" stage to prepare containerFactory, memberTag and usercontext
        private AdminkaRoutineHandlerBaseAsync2(
            ApplicationSettings applicationSettings,
            IPerformanceCounters performanceCounters,
            Func<Exception, Guid, MemberTag, Exception> routineTransformException,
            Func<Guid, MemberTag, IMemberLogger> composeLoggers,
            IContainer container,
            Guid correaltionToken,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input
        ) : this(
                applicationSettings,
                new AdminkaRoutineHandlerFactory<TUserContext>(correaltionToken,
                    routineTransformException,
                    composeLoggers,
                    performanceCounters),
                container,
                memberTag,
                userContext,
                hasVerboseLoggingPrivilege,
                getAuditStamp,
                input)
        {
        }
        #endregion

        // Used as "pre-final-1"  stage to prepare container
        private AdminkaRoutineHandlerBaseAsync2(
            ApplicationSettings applicationSettings,
            AdminkaRoutineHandlerFactory<TUserContext> routineLogger,
            IContainer container,
            MemberTag memberTag,
            TUserContext userContext,
            bool hasVerboseLoggingPrivilege,
            Func<TUserContext, string> getAuditStamp,
            object input) :
            this(
                applicationSettings,
                getAuditStamp,
                routineLogger.CreateLoggingHandler(memberTag, container, userContext,
                    hasVerboseLoggingPrivilege,
                    input)
                )
        {
        }

        // final
        public AdminkaRoutineHandlerBaseAsync2(
            ApplicationSettings applicationSettings,
            Func<TUserContext, string> getAuditStamp,
            IHandlerAsync<RoutineClosure<TUserContext>> routineHandler
        ) : base(
                closure => Task.Run(()=>new PerCallContainer<TUserContext>(closure, applicationSettings, getAuditStamp)),
                routineHandler
            )
        {
        }
    }
*/
}