using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ProcessManager
    {
        public CreateProcessOutput CreateNewProcess(CreateProcessInput createProcessInput)
        {
            string serializedInput = Vanrise.Common.Serializer.Serialize(createProcessInput);
            CreateProcessOutput output = null;            
            CreateServiceClient((client) =>
                {
                    output = client.CreateNewProcess(serializedInput);
                });
            return output;
        }

        private void CreateServiceClient(Action<IBPService> onClientReady)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            binding.OpenTimeout = TimeSpan.FromMinutes(5);
            binding.CloseTimeout = TimeSpan.FromMinutes(5);
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            
            ChannelFactory<IBPService> channelFactory = new ChannelFactory<IBPService>(binding, new EndpointAddress("net.pipe://localhost/BPService"));
            IBPService client = channelFactory.CreateChannel();
            try
            {
                onClientReady(client);
            }
            finally
            {
                (client as IDisposable).Dispose();
            }
        }

        public TriggerProcessEventOutput TriggerProcessEvent(TriggerProcessEventInput triggerProcessEventInput)
        {
            string serializedInput = Vanrise.Common.Serializer.Serialize(triggerProcessEventInput);
            TriggerProcessEventOutput output = null;
            CreateServiceClient((client) =>
            {
                output = client.TriggerProcessEvent(serializedInput);
            });
            return output;
        }

        public List<BPDefinition> GetDefinitions()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitions();
        }
    }
}
