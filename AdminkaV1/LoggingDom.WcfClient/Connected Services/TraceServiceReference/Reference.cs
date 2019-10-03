﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


namespace TraceServiceReference
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RoutineError", Namespace="https://adminka-v1.dashboardcode.com")]
    internal partial class RoutineError : object
    {
        
        private string AdminkaExceptionCodeField;
        
        private System.Guid CorrelationTokenField;
        
        private System.Collections.Generic.Dictionary<string, string> DataField;
        
        private string DetailsField;
        
        private TraceServiceReference.MemberTag MemberTagField;
        
        private string MessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string AdminkaExceptionCode
        {
            get
            {
                return this.AdminkaExceptionCodeField;
            }
            set
            {
                this.AdminkaExceptionCodeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal System.Guid CorrelationToken
        {
            get
            {
                return this.CorrelationTokenField;
            }
            set
            {
                this.CorrelationTokenField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal System.Collections.Generic.Dictionary<string, string> Data
        {
            get
            {
                return this.DataField;
            }
            set
            {
                this.DataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Details
        {
            get
            {
                return this.DetailsField;
            }
            set
            {
                this.DetailsField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal TraceServiceReference.MemberTag MemberTag
        {
            get
            {
                return this.MemberTagField;
            }
            set
            {
                this.MemberTagField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MemberTag", Namespace="https://adminka-v1.dashboardcode.com")]
    internal partial class MemberTag : object
    {
        
        private string MemberField;
        
        private string NamespaceField;
        
        private string TypeField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Member
        {
            get
            {
                return this.MemberField;
            }
            set
            {
                this.MemberField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Namespace
        {
            get
            {
                return this.NamespaceField;
            }
            set
            {
                this.NamespaceField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Type
        {
            get
            {
                return this.TypeField;
            }
            set
            {
                this.TypeField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthenticationFault", Namespace="https://adminka-v1.dashboardcode.com")]
    internal partial class AuthenticationFault : object
    {
        
        private string MessageField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        internal string Message
        {
            get
            {
                return this.MessageField;
            }
            set
            {
                this.MessageField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://adminka-v1.dashboardcode.com/TraceService", ConfigurationName="TraceServiceReference.TraceService")]
    internal interface TraceService
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTrace", ReplyAction="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTraceResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(TraceServiceReference.RoutineError), Action="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTraceRoutineErr" +
            "orFault", Name="RoutineError", Namespace="https://adminka-v1.dashboardcode.com")]
        [System.ServiceModel.FaultContractAttribute(typeof(TraceServiceReference.AuthenticationFault), Action="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTraceAuthentica" +
            "tionFaultFault", Name="AuthenticationFault", Namespace="https://adminka-v1.dashboardcode.com")]
        System.Collections.Generic.List<DashboardCode.AdminkaV1.LoggingDom.VerboseRecord> GetTrace(System.Guid correlationToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTrace", ReplyAction="https://adminka-v1.dashboardcode.com/TraceService/TraceService/GetTraceResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<DashboardCode.AdminkaV1.LoggingDom.VerboseRecord>> GetTraceAsync(System.Guid correlationToken);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    internal interface TraceServiceChannel : TraceServiceReference.TraceService, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Tools.ServiceModel.Svcutil", "2.0.1")]
    internal partial class TraceServiceClient : System.ServiceModel.ClientBase<TraceServiceReference.TraceService>, TraceServiceReference.TraceService
    {
        
        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public TraceServiceClient() : 
                base(TraceServiceClient.GetDefaultBinding(), TraceServiceClient.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_TraceService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TraceServiceClient(EndpointConfiguration endpointConfiguration) : 
                base(TraceServiceClient.GetBindingForEndpoint(endpointConfiguration), TraceServiceClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TraceServiceClient(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(TraceServiceClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TraceServiceClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(TraceServiceClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public TraceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Collections.Generic.List<DashboardCode.AdminkaV1.LoggingDom.VerboseRecord> GetTrace(System.Guid correlationToken)
        {
            return base.Channel.GetTrace(correlationToken);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<DashboardCode.AdminkaV1.LoggingDom.VerboseRecord>> GetTraceAsync(System.Guid correlationToken)
        {
            return base.Channel.GetTraceAsync(correlationToken);
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_TraceService))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_TraceService))
            {
                return new System.ServiceModel.EndpointAddress("http://localhost:64220/TraceService.svc");
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return TraceServiceClient.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_TraceService);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return TraceServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_TraceService);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_TraceService,
        }
    }
}
