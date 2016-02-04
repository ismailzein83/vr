using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueItemManager
    {
       

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            IQueueItemDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            return manager.GetItemStatusSummary();
        
        }




        public IEnumerable<ExecutionFlowStatusSummary> GetExecutionFlowStatusSummary()
        {
            QueueItemManager queueItemManager = new QueueItemManager();
            QueueInstanceManager queueInstanceManager = new QueueInstanceManager();
            List<QueueItemStatusSummary> queueItemStatusSummary = queueItemManager.GetItemStatusSummary();
            IEnumerable<QueueInstance> queueInstances = queueInstanceManager.GetAllQueueInstances();

            IEnumerable<ExecutionFlowStatusSummary> result = from qInstances in queueInstances
                                                             join qItemStatusSummary in queueItemStatusSummary
                                                             on qInstances.QueueInstanceId equals qItemStatusSummary.QueueId
                                                             group new { qItemStatusSummary, qInstances, qItemStatusSummary.Count }
                                                             by new { qItemStatusSummary, qItemStatusSummary.Status, qInstances.ExecutionFlowId } into res
                                                             select new ExecutionFlowStatusSummary
                                                             {
                                                                 ExecutionFlowId = (int)res.Key.ExecutionFlowId,
                                                                 Status = res.Key.Status,
                                                                 Count = res.Key.qItemStatusSummary.Count
                                                             };



            IEnumerable<ExecutionFlowStatusSummary> filteredResult = from c in result
                                                                     group c by new { c.ExecutionFlowId, c.Status } into item
                                                                     select new ExecutionFlowStatusSummary
                                                                     {
                                                                         ExecutionFlowId = item.Key.ExecutionFlowId,
                                                                         Status = item.Key.Status,
                                                                         Count = item.Sum(x => x.Count)
                                                                     };

            return filteredResult;
        }


    }
}
