﻿using System;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.Configuration
{
    public class LoggingVerboseConfiguration :IProgress<string>
    {
        public bool UseBufferForVerbose { get; private set; } = true;
        public bool VerboseWithStackTrace { get; private set; } = false;
        public void Report(string json)
        {
            if (json != null)
            {
                var dictionary = InjectedManager.DeserializeJson<Dictionary<string, string>>(json);
                UseBufferForVerbose = bool.Parse(dictionary["UseBufferForVerbose"]);
                string verboseWithStackTrace;
                if (dictionary.TryGetValue("VerboseWithStackTrace", out verboseWithStackTrace))
                {
                    VerboseWithStackTrace = bool.Parse(verboseWithStackTrace);
                }
            }
        }
    }
}
