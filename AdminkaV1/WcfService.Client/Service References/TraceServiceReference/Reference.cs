﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RoutineError", Namespace="https://dashboardcode.com/Adminka-V1")]
    [System.SerializableAttribute()]
    public partial class RoutineError : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DetailsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.RoutineTag RoutineTagField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UserContextExceptionCodeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Details {
            get {
                return this.DetailsField;
            }
            set {
                if ((object.ReferenceEquals(this.DetailsField, value) != true)) {
                    this.DetailsField = value;
                    this.RaisePropertyChanged("Details");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.RoutineTag RoutineTag {
            get {
                return this.RoutineTagField;
            }
            set {
                if ((object.ReferenceEquals(this.RoutineTagField, value) != true)) {
                    this.RoutineTagField = value;
                    this.RaisePropertyChanged("RoutineTag");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UserContextExceptionCode {
            get {
                return this.UserContextExceptionCodeField;
            }
            set {
                if ((object.ReferenceEquals(this.UserContextExceptionCodeField, value) != true)) {
                    this.UserContextExceptionCodeField = value;
                    this.RaisePropertyChanged("UserContextExceptionCode");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RoutineTag", Namespace="https://dashboardcode.com/Adminka-V1")]
    [System.SerializableAttribute()]
    public partial class RoutineTag : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Guid CorrelationTokenField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MemberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string NamespaceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TypeField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Guid CorrelationToken {
            get {
                return this.CorrelationTokenField;
            }
            set {
                if ((this.CorrelationTokenField.Equals(value) != true)) {
                    this.CorrelationTokenField = value;
                    this.RaisePropertyChanged("CorrelationToken");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Member {
            get {
                return this.MemberField;
            }
            set {
                if ((object.ReferenceEquals(this.MemberField, value) != true)) {
                    this.MemberField = value;
                    this.RaisePropertyChanged("Member");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Namespace {
            get {
                return this.NamespaceField;
            }
            set {
                if ((object.ReferenceEquals(this.NamespaceField, value) != true)) {
                    this.NamespaceField = value;
                    this.RaisePropertyChanged("Namespace");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Type {
            get {
                return this.TypeField;
            }
            set {
                if ((object.ReferenceEquals(this.TypeField, value) != true)) {
                    this.TypeField = value;
                    this.RaisePropertyChanged("Type");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AuthenticationFault", Namespace="https://dashboardcode.com/Adminka-V1")]
    [System.SerializableAttribute()]
    public partial class AuthenticationFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://DashboardCode.com/Adminka-V1", ConfigurationName="TraceServiceReference.TraceService")]
    public interface TraceService {
        
        [System.ServiceModel.OperationContractAttribute(Action="https://DashboardCode.com/Adminka-V1/TraceService/GetTrace", ReplyAction="https://DashboardCode.com/Adminka-V1/TraceService/GetTraceResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.RoutineError), Action="https://DashboardCode.com/Adminka-V1/TraceService/GetTraceRoutineErrorFault", Name="RoutineError", Namespace="https://dashboardcode.com/Adminka-V1")]
        [System.ServiceModel.FaultContractAttribute(typeof(DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.AuthenticationFault), Action="https://DashboardCode.com/Adminka-V1/TraceService/GetTraceAuthenticationFaultFaul" +
            "t", Name="AuthenticationFault", Namespace="https://dashboardcode.com/Adminka-V1")]
        DashboardCode.AdminkaV1.DomLogging.Trace GetTrace(System.Guid correlationToken);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://DashboardCode.com/Adminka-V1/TraceService/GetTrace", ReplyAction="https://DashboardCode.com/Adminka-V1/TraceService/GetTraceResponse")]
        System.Threading.Tasks.Task<DashboardCode.AdminkaV1.DomLogging.Trace> GetTraceAsync(System.Guid correlationToken);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface TraceServiceChannel : DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.TraceService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TraceServiceClient : System.ServiceModel.ClientBase<DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.TraceService>, DashboardCode.AdminkaV1.WcfService.Client.TraceServiceReference.TraceService {
        
        public TraceServiceClient() {
        }
        
        public TraceServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public TraceServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TraceServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public TraceServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DashboardCode.AdminkaV1.DomLogging.Trace GetTrace(System.Guid correlationToken) {
            return base.Channel.GetTrace(correlationToken);
        }
        
        public System.Threading.Tasks.Task<DashboardCode.AdminkaV1.DomLogging.Trace> GetTraceAsync(System.Guid correlationToken) {
            return base.Channel.GetTraceAsync(correlationToken);
        }
    }
}
