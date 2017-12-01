﻿using System;
using System.ServiceModel;
using System.Text;
using DashboardCode.Routines;
using TraceServiceReference;

namespace DashboardCode.AdminkaV1.LoggingDom.WcfClient
{
    // TODO: this code can be generated with T4
    public class TraceServiceProxy : ITraceService
    {
        public Trace GetTrace(Guid correlationToken)
        {
            var client = new TraceServiceClient();
            try
            {
                var task = client.GetTraceAsync(correlationToken);
                var trace = task.Result;
                return trace;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count > 0 && ex.InnerExceptions[0] is FaultException<RoutineError> faultException)
                {
                    if (faultException.Detail.UserContextExceptionCode != null)
                    {
                        var baseException = new UserContextException(faultException.Message, faultException, faultException.Detail.UserContextExceptionCode);
                        baseException.CopyData(faultException.Detail.Data);
                        throw baseException;
                    }
                    else
                    {
                        ex.CopyData(faultException.Detail.Data);
                    }
                }
                throw;
            }
        }

        internal static void AppendFaultException(StringBuilder stringBuilder, FaultException<RoutineError> exception)
        {
            var routineError = exception.Detail;
            stringBuilder.AppendMarkdownLine(nameof(FaultException<RoutineError>) + " specific:");
            stringBuilder.Append("   ").AppendMarkdownProperty("FaultCode.Name", exception.Code.Name);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.UserContextExceptionCode", routineError.UserContextExceptionCode);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.CorrelationToken", routineError.RoutineGuid.CorrelationToken.ToString());
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Namespace", routineError.RoutineGuid.Namespace);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Type", routineError.RoutineGuid.Type);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.RoutineGuid.Member", routineError.RoutineGuid.Member);
            stringBuilder.Append("   ").AppendMarkdownProperty("RoutineError.Details", routineError.Details);
        }

        public static void AppendWcfClientFaultException(StringBuilder stringBuilder, Exception exception)
        {
            if (exception is FaultException<RoutineError> faultException)
                AppendFaultException(stringBuilder, faultException);
        }
    }
}