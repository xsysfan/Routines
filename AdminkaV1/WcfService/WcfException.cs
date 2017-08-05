﻿using System;
using System.ServiceModel;
using DashboardCode.AdminkaV1.Wcf.Messaging.Contracts;

namespace DashboardCode.AdminkaV1.Wcf.Messaging
{
    [Serializable]
    public class WcfException : FaultException<RoutineError>
    {
        public WcfException(
            RoutineError routineError,
            FaultReason reason,
            FaultCode code)
            : base(routineError, reason, code)
        {
        }

        public static Exception TransformException(Exception exception, Routines.RoutineGuid routineGuid, string faultCodeNamespace, Func<Exception, string> markdownException)
        {
            var message = default(string);
            var code = default(string);
            if (exception is UserContextException)
            {
                message = exception.Message;
                code = ((UserContextException)exception).Code;
            }
            else
            {
                message = "Remote server error: " + exception.Message + "(" + exception.GetType().FullName + ")";
                if (exception.Data.Contains("Code"))
                    code = exception.Data["Code"] as string;
            }

            var routineError = new RoutineError()
            {
                RoutineGuid = new Contracts.RoutineGuid()
                {
                    CorrelationToken = routineGuid.CorrelationToken,
                    Namespace = routineGuid.MemberTag.Namespace,
                    Type = routineGuid.MemberTag.Type,
                    Member = routineGuid.MemberTag.Member
                },
                Message = message,
                UserContextExceptionCode = code,
                Details = markdownException(exception)
            };

            return new WcfException(routineError, new FaultReason(message),
                new FaultCode("UNSPECIFIED", faultCodeNamespace));
        }
    }
}