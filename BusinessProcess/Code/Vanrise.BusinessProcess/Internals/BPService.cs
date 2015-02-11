using System;
using System.Collections.Generic;
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
        internal static void Start()
        {
            ServiceHost serviceHost = new ServiceHost(typeof(BPService));
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.MaxReceivedMessageSize = int.MaxValue;
            serviceHost.AddServiceEndpoint(typeof(IBPService), binding, "net.pipe://localhost/BPService");
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += new EventHandler(serviceHost_Opened);
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
            serviceHost.Open();
        }

        static void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) is opening..");
        }

        static void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) Opened");
        }

        static void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) Closed");
        }

        static void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Business Process WCF Service (BPService) is closing..");
        }

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
    }
}
