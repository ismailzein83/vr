using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;
using System.Configuration;

namespace Vanrise.Queueing
{
    public class QueueItemManager
    {
        static QueueItemManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Queueing_GetItemStatusSummaryTimeInterval"], out s_GetItemStatusSummaryTimeInterval))
                s_GetItemStatusSummaryTimeInterval = new TimeSpan(0, 0, 2);
        }

        private static TimeSpan s_GetItemStatusSummaryTimeInterval;

        private static DateTime s_lastTimeCalled = new DateTime();

        private static List<QueueItemStatusSummary> s_itemStatusSummary;

        private static Object s_thisLock = new Object();

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
            {
                lock (s_thisLock)
                {
                    if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
                    {
                        IQueueItemDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
                        s_itemStatusSummary = manager.GetItemStatusSummary();
                        s_lastTimeCalled = DateTime.Now;
                    }
                }
            }
            return s_itemStatusSummary;

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
