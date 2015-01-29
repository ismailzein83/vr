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
        #region Process Workflow Methods

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

        #endregion

        #region BP Transaction Methods

        public List<BPDefinition> GetDefinitions()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitions();
        }

        public T GetDefinitionObjectState<T>(int definitionId, string objectKey)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitionObjectState<T>(definitionId, objectKey);
        }

        public void SaveDefinitionObjectState(int definitionId, string objectKey, object objectValue)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            if (dataManager.UpdateDefinitionObjectState(definitionId, objectKey, objectValue) <= 0)
                dataManager.InsertDefinitionObjectState(definitionId, objectKey, objectValue);
        }

        public List<BPInstance> GetFilteredInstances(int definitionID, DateTime datefrom, DateTime dateto)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetInstancesByCriteria(definitionID, datefrom, dateto);
        }

        public List<BPTrackingMessage> GetTrackingsByInstanceId(long processInstanceID)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return dataManager.GetTrackingsByInstanceId(processInstanceID);
        }


        #endregion

        #region Private Methods

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

        #endregion

    }
}
