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
            QueueInstanceManager queuegManager = new QueueInstanceManager();
            IEnumerable<QueueInstance> allQueues = queuegManager.GetReadyQueueInstances();
            foreach (var queueInstance in allQueues)
            {
                if (queueInstance.Settings != null && queueInstance.Settings.Activator != null)
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
                            var queue = PersistentQueueFactory.Default.GetQueue(queueInstance.Name);
                            if (queueInstance.ExecutionFlowId != null)
                            {
                                QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
                                var queuesByStages = executionFlowManager.GetQueuesByStages(queueInstance.ExecutionFlowId.Value);
                                while (queue.TryDequeueObject((itemToProcess) =>
                                {
                                    QueueActivatorExecutionContext context = new QueueActivatorExecutionContext(itemToProcess, queueInstance);
                                    queueInstance.Settings.Activator.ProcessItem(context);
                                    if (context.OutputItems != null && context.OutputItems.Count > 0)
                                    {
                                        foreach (var outputItem in context.OutputItems)
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
