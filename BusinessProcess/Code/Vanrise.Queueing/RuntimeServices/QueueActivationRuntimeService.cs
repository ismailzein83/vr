using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace Vanrise.Queueing
{
    public class QueueActivationRuntimeService : RuntimeService
    {
        static ServiceHost s_serviceHost;
        static Object s_hostServiceLockObject = new object();
        static string s_serviceURL;

        protected override void OnStarted(IRuntimeServiceStartContext context)
        {
            HostServiceIfNeeded();
            RegisterActivator();
            base.OnStarted(context);
        }

        Guid _activatorId;

        private void HostServiceIfNeeded()
        {
           lock(s_hostServiceLockObject)
           {
               if(s_serviceHost == null)
               {
                   s_serviceHost = ServiceHostManager.Current.CreateAndOpenTCPServiceHost(typeof(QueueActivationRuntimeWCFService), typeof(IQueueActivationRuntimeWCFService), OnServiceHostCreated, OnServiceHostRemoved, out s_serviceURL);
               }
           }
        }

        private void RegisterActivator()
        {
            _activatorId = Guid.NewGuid();
            var dataManager = QDataManagerFactory.GetDataManager<IQueueActivatorInstanceDataManager>();
            dataManager.InsertActivator(_activatorId, Vanrise.Runtime.RunningProcessManager.CurrentProcess.ProcessId, QueueActivatorType.Normal, s_serviceURL);
        }

        IQueueItemDataManager _queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
        QueueInstanceManager _queueManager = new QueueInstanceManager();
        QueueExecutionFlowManager _executionFlowManager = new QueueExecutionFlowManager();
        protected override void Execute()
        {
            int queueIdToProcess;
            while (QueueActivationRuntimeWCFService.TryGetPendingQueueToProcess(this._activatorId, out queueIdToProcess))
            {
                QueueInstance queueInstance = _queueManager.GetQueueInstanceById(queueIdToProcess);
                if (queueInstance == null)
                    throw new NullReferenceException(String.Format("queueInstance. {0}", queueIdToProcess));
                IPersistentQueue queue = PersistentQueueFactory.Default.GetQueue(queueIdToProcess);
                var queuesByStages = _executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                var dequeueContext = new PersistentQueueDequeueContext { ActivatorInstanceId = this._activatorId };
                bool hasItem = false;
                do
                {
                    Action<PersistentQueueItem> processItem =
                        (itemToProcess) =>
                        {
                            QueueActivatorExecutionContext context = new QueueActivatorExecutionContext(itemToProcess, queueInstance);
                            queueInstance.Settings.Activator.ProcessItem(context);

                            if (context.OutputItems != null && context.OutputItems.Count > 0)
                            {
                                if (queueInstance.ExecutionFlowId.HasValue)
                                {
                                    foreach (var outputItem in context.OutputItems)
                                    {
                                        outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
                                        var outputQueue = queuesByStages[outputItem.StageName].Queue;
                                        outputQueue.EnqueueObject(outputItem.Item);
                                    }
                                }
                                else
                                    throw new NullReferenceException(String.Format("queueInstance.ExecutionFlowId. Queue Instance ID {0}", queueInstance.QueueInstanceId));
                            }
                        };
                    hasItem = queue.TryDequeueObject(processItem, dequeueContext);
                }
                while (hasItem);
            }
        }


        void OnServiceHostCreated(ServiceHost serviceHost)
        {
            serviceHost.Opening += serviceHost_Opening;
            serviceHost.Opened += serviceHost_Opened;
            serviceHost.Closing += serviceHost_Closing;
            serviceHost.Closed += serviceHost_Closed;
        }

        void OnServiceHostRemoved(ServiceHost serviceHost)
        {
            serviceHost.Opening -= serviceHost_Opening;
            serviceHost.Opened -= serviceHost_Opened;
            serviceHost.Closing -= serviceHost_Closing;
            serviceHost.Closed -= serviceHost_Closed;
        }

        void serviceHost_Opening(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Queue Activation WCF Service is opening..");
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Queue Activation WCF Service opened");
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Queue Activation WCF Service closed");
        }

        void serviceHost_Closing(object sender, EventArgs e)
        {
            LoggerFactory.GetLogger().WriteInformation("Queue Activation WCF Service is closing..");
        }

    }
}
