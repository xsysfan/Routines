﻿using Microsoft.Extensions.Configuration;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETStandard;
using Xunit;

[assembly: CollectionBehavior(MaxParallelThreads = 1, DisableTestParallelization = true)]

namespace DashboardCode.AdminkaV1.Injected.NETStandard.Test
{
    public class ConfigurationNETStandard : IAppConfiguration
    {
        public IConfigurationRoot ConfigurationRoot { get; private set; }
        public ConfigurationNETStandard()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json", false, true); // false indicates file is not optional
            this.ConfigurationRoot = configurationBuilder.Build();
        }
        public SpecifiableConfigurationContainer GetConfigurationContainer(string @namespace, string @class, string member)
        {
            return RoutinesConfigurationManager.GetConfigurationContainer(ConfigurationRoot, @namespace, @class, member);
        }

        public string GetConnectionString()
        {
            return RoutinesConfigurationManager.GetConnectionString(ConfigurationRoot, "adminka");
        }
    }
}
