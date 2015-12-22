using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using Vanrise.Runtime;

namespace Vanrise.Queueing
{
    public class QueueActivationService : RuntimeService
    {
        static int s_runningThreads;
        static int s_maxNumberOfConcurrentThreads;
        static object s_lockObj = new object();

        static QueueActivationService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["QueueActivationService_MaxThreadsCount"], out s_maxNumberOfConcurrentThreads))
                s_maxNumberOfConcurrentThreads = 25;
        }

        protected override void Execute()
        {
            QueueingManager queueingManager = new QueueingManager();
            IEnumerable<QueueInstance> allQueues = queueingManager.GetReadyQueueInstances();
            foreach (var queueInstance in allQueues)
            {
                if (queueInstance.Settings != null && !String.IsNullOrEmpty(queueInstance.Settings.QueueActivatorFQTN))
                {
                    while (s_runningThreads >= s_maxNumberOfConcurrentThreads)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    lock (s_lockObj)
                        s_runningThreads++;
                    Task task = new Task(() =>
                    {
                        try
                        {
                            Type queueActivatorType = Type.GetType(queueInstance.Settings.QueueActivatorFQTN);
                            if (queueActivatorType == null)
                                throw new Exception(String.Format("Could not load QueueActivator type '{0}'", queueInstance.Settings.QueueActivatorFQTN));
                            QueueActivator queueActivator = Activator.CreateInstance(queueActivatorType) as QueueActivator;
                            if (queueActivator == null)
                                throw new Exception(String.Format("'{0}' is not of type Vanrise.Queueing.Entities.QueueActivator", queueInstance.Settings.QueueActivatorFQTN));
                            using (queueActivator)
                            {
                                var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
                                if (queueInstance.ExecutionFlowId != null)
                                {
                                    QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
                                    var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                                    while (queue.TryDequeueObject((itemToProcess) =>
                                    {
                                        ItemsToEnqueue outputItems = new ItemsToEnqueue();
                                        queueActivator.ProcessItem(itemToProcess, outputItems);
                                        if (outputItems.Count > 0)
                                        {
                                            foreach (var outputItem in outputItems)
                                            {
                                                outputItem.Item.ExecutionFlowTriggerItemId = itemToProcess.ExecutionFlowTriggerItemId;
                                                queuesByStages[outputItem.StageName].Queue.EnqueueObject(outputItem.Item);
                                            }
                                        }
                                    }))
                                    {

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                        finally
                        {
                            lock (s_lockObj)
                                s_runningThreads--;
                        }
                    });
                    task.Start();
                }
            }
        }
    }
}
