﻿using System;
using System.Linq;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Storage;
using DashboardCode.Routines.Logging;

namespace DashboardCode.AdminkaV1.Injected.Logging
{
    public class ListLoggingAdapter : IMemberLogger, IUnhandledExceptionLogging
    {
        readonly List<string> logger = new List<string>();
        readonly Guid correlationToken;
        readonly MemberTag memberTag;
        readonly Func<Exception, string> markdownException;
        readonly Func<object, string> serializeObject;
        readonly string routineTag;

        public ListLoggingAdapter(
            List<string> logger,
            Guid correlationToken,
            MemberTag memberTag,
            Func<Exception, string> markdownException,
            Func<object, int, bool, string> serializeObject
            )
        {
            this.logger = logger;
            this.correlationToken = correlationToken;
            this.memberTag = memberTag;
            this.routineTag = memberTag.ToText(correlationToken);
            this.markdownException = markdownException;
            this.serializeObject = (o) =>
            {
                try
                {
                    return serializeObject(o, 1, true);
                }
                catch (Exception ex)
                {
                    LogException(DateTime.Now, ex);
                    return null;
                }
            };
        }
        
        public void LogActivityStart(DateTime dateTime)
        {
            var text = $"LogActivityStart, {dateTime} {routineTag}";
            logger.Add(text);
        }
        public void LogActivityFinish(DateTime dateTime, TimeSpan timeSpan, bool isSuccess)
        {
            var text = $"LogActivityFinish, {dateTime} {routineTag} duration: {Math.Round(timeSpan.TotalMilliseconds)}ms{(isSuccess ? "" : ", #ERROR")}";
            logger.Add(text);
        }
        public void LogVerbose(DateTime dateTime, string message)
        {
            var text = $"LogVerbose, {dateTime} {routineTag} message: {message}";
            logger.Add(text);
        }
        public void LogBufferedVerbose(IEnumerable<VerboseMessage> verboseMessages)
        {
            var count = verboseMessages.Count();
            var i = 1;
            foreach (var verbose in verboseMessages)
            {
                var text = $"LogBufferedVerbose {i++}/{count}, {verbose.DateTime} {routineTag} message: {verbose.Message}";
                if (verbose.StackTrace != null)
                    text += Environment.NewLine + "StackTrace: " + verbose.StackTrace;
                logger.Add(text);
            }
        }

        public void LogException(DateTime dateTime, Exception excepion)
        {
            var message = markdownException(excepion);
            var text = $"LogException, {dateTime} {routineTag} message:{Environment.NewLine}{message}";
            logger.Add(text);
        }

        public void LogError(DateTime dateTime, string message)
        {
            var text = $"LogError, {dateTime} {routineTag} message:{Environment.NewLine}{message}";
            logger.Add(text);
        }

        public void Input(DateTime dateTime, object input)
        {
            var message = serializeObject(input);
            var text = $"Input, {dateTime} {routineTag} message: {message}";
            logger.Add(text);
        }

        public void Output(DateTime dateTime, object output)
        {
            var message = serializeObject(output);
            var text = $"Output, {dateTime} {routineTag} message: {message}";
            logger.Add(text);
        }

        public void TraceError(Guid correlationToken, string message)
        {
            logger.Add($"{correlationToken} {message}");
        }
    }
}