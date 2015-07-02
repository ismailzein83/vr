﻿using System;
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
            if (createProcessInput == null)
                throw new ArgumentNullException("createProcessInput");
            if (createProcessInput.InputArguments == null)
                throw new ArgumentNullException("createProcessInput.InputArguments");

            BPDefinition processDefinition = GetDefinition(createProcessInput.InputArguments.ProcessName);
            if (processDefinition == null)
                throw new Exception(String.Format("No Process Definition found match with input argument '{0}'", createProcessInput.InputArguments.GetType()));

            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            string processTitle = createProcessInput.InputArguments.GetTitle();
            long processInstanceId = dataManager.InsertInstance(processTitle, createProcessInput.ParentProcessID, processDefinition.BPDefinitionID, createProcessInput.InputArguments, BPInstanceStatus.New);
            IBPTrackingDataManager dataManagerTracking = BPDataManagerFactory.GetDataManager<IBPTrackingDataManager>();
            dataManagerTracking.Insert(new BPTrackingMessage
            {
                ProcessInstanceId = processInstanceId,
                ParentProcessId = createProcessInput.ParentProcessID,
                Message = String.Format("Process Created: {0}", processTitle),
                Severity = BPTrackingSeverity.Information,
                EventTime = DateTime.Now
            });
            CreateProcessOutput output = new CreateProcessOutput
            {
                ProcessInstanceId = processInstanceId,
                Result = CreateProcessResult.Succeeded
            };
            return output;
        }

        public BPDefinition GetDefinition(string processName)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            List<BPDefinition> definitions = dataManager.GetDefinitions();
            return definitions.FirstOrDefault(itm => itm.Name == processName);
        }

        public TriggerProcessEventOutput TriggerProcessEvent(TriggerProcessEventInput triggerProcessEventInput)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            TriggerProcessEventOutput output = new TriggerProcessEventOutput();
            if (dataManager.InsertEvent(triggerProcessEventInput.ProcessInstanceId, triggerProcessEventInput.BookmarkName, triggerProcessEventInput.EventData) > 0)
                output.Result = TriggerProcessEventResult.Succeeded;
            else
                output.Result = TriggerProcessEventResult.ProcessInstanceNotExists;
            return output;
        }

        #endregion

        #region BP Transaction Methods

        public List<BPInstance> GetOpenedInstances()
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetOpenedInstances();
        }
        public List<BPDefinition> GetFilteredDefinitions(string title)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetFilteredDefinitions(title);
        }

        public BPDefinition GetDefinition(int ID)
        {
            IBPDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPDataManager>();
            return dataManager.GetDefinition(ID);
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
