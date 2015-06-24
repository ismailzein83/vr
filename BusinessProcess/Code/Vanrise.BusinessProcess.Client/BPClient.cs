using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess.Client
{
    public  partial class  BPClient
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

        public List<BPInstance> GetOpenedInstances()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetOpenedInstances();
        }
        public List<BPDefinition> GetFilteredDefinitions(int fromRow, int toRow, string title)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetFilteredDefinitions(fromRow, toRow, title);
        }

        public List<BPDefinition> GetDefinitions()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinitions();
        }

        public List<BPInstance> GetFilteredInstances(int definitionID, DateTime dateFrom, DateTime dateTo)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetInstancesByCriteria(definitionID, dateFrom, dateTo);
        }

        public List<BPInstance> GetFilteredInstances(List<int> definitionID, List<BPInstanceStatus> instanceStatus, DateTime dateFrom, DateTime dateTo)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetInstancesByCriteria(definitionID, instanceStatus, dateFrom, dateTo);
        }

        public BPInstance GetInstance(long instanceId)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetInstance(instanceId);
        }

        public List<BPTrackingMessage> GetTrackingsByInstanceId(long processInstanceID, long lastTrackingId)
        {
            IBPTrackingDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            return dataManager.GetTrackingsByInstanceId(processInstanceID,  lastTrackingId);
        }


        #endregion

        #region Private Methods

        private void CreateServiceClient(Action<IBPService> onClientReady)
        {
            Binding binding;
            string serviceURL;

            string tcpServiceHost = ConfigurationManager.AppSettings["BusinessProcessServiceHost"];
            if (!String.IsNullOrEmpty(tcpServiceHost))
            {
                binding = new NetTcpBinding(SecurityMode.None)
                {
                    MaxBufferPoolSize = int.MaxValue,
                    MaxBufferSize = int.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue
                };
                serviceURL = String.Format("net.tcp://{0}/BPService", tcpServiceHost);
            }
            else
            {
                binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None)
                    {
                        MaxBufferPoolSize = int.MaxValue,
                        MaxBufferSize = int.MaxValue,
                        MaxReceivedMessageSize = int.MaxValue
                    };
                serviceURL = "net.pipe://localhost/BPService";
            }

            binding.OpenTimeout = TimeSpan.FromMinutes(5);
            binding.CloseTimeout = TimeSpan.FromMinutes(5);
            binding.SendTimeout = TimeSpan.FromMinutes(5);
            binding.ReceiveTimeout = TimeSpan.FromMinutes(5);

            ChannelFactory<IBPService> channelFactory = new ChannelFactory<IBPService>(binding, new EndpointAddress(serviceURL));
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
