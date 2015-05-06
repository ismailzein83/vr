using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace Vanrise.BusinessProcess
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BPService" in both code and config file together.
    internal class BPService : IBPService
    {
        #region WCF Service

        internal static void Start()
        {
            StartService();
            
        }

        private static void StartService()
        {
            ServiceHost serviceHost = new ServiceHost(typeof(BPService));
            if (ConfigurationManager.AppSettings["BusinessProcessDisabledNamedPipeService"] != "true")
                AddNamedPipeEndPoint(serviceHost);
            string tcpServiceHost = ConfigurationManager.AppSettings["BusinessProcessServiceHost"];
            if (!String.IsNullOrEmpty(tcpServiceHost))
                AddTCPEndPoint(serviceHost, tcpServiceHost);
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
            serviceHost.Open();
        }

        private static void AddNamedPipeEndPoint(ServiceHost serviceHost)
        {
            LoggerFactory.GetLogger().WriteInformation("Adding NetNamedPipe End Point to Business Process WCF Service (BPService)...");
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            serviceHost.AddServiceEndpoint(typeof(IBPService), binding, "net.pipe://localhost/BPService");
            LoggerFactory.GetLogger().WriteInformation("NetNamedPipe End Point to Business Process WCF Service (BPService) added");
        }

        private static void AddTCPEndPoint(ServiceHost serviceHost, string tcpServiceHost)
        {
            LoggerFactory.GetLogger().WriteInformation("Adding NetTcp End Point to Business Process WCF Service (BPService)...");
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            serviceHost.AddServiceEndpoint(typeof(IBPService), binding, String.Format("net.tcp://{0}/BPService", tcpServiceHost));
            LoggerFactory.GetLogger().WriteInformation("NetTcp End Point to Business Process WCF Service (BPService) added");
        }
        
        static void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) is opening..");
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) opened");
        }

        static void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) closed");
        }

        static void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) is closing..");
        }

        #endregion

        public CreateProcessOutput CreateNewProcess(string serializedInput)
        {
            CreateProcessInput input = Serializer.Deserialize(serializedInput) as CreateProcessInput;
            if (input == null)
                throw new ArgumentException("CreateProcessInput");
            return BusinessProcessRuntime.Current.CreateNewProcess(input);
        }
        
        public TriggerProcessEventOutput TriggerProcessEvent(string serializedInput)
        {
            TriggerProcessEventInput input = Serializer.Deserialize(serializedInput) as TriggerProcessEventInput;
            if (input == null)
                throw new ArgumentException("TriggerProcessEventInput");
            return BusinessProcessRuntime.Current.TriggerProcessEvent(input);
        }

        public BPInstance GetInstance(long processInstanceId)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetInstance(processInstanceId);
        }
    }
}
