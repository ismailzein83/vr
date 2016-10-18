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
    public class QueueItemHeaderManager
    {


        #region ctor/Local Variables

        static QueueItemHeaderManager()
        {
            if (!TimeSpan.TryParse(ConfigurationManager.AppSettings["Queueing_GetItemStatusSummaryTimeInterval"], out s_GetItemStatusSummaryTimeInterval))
                s_GetItemStatusSummaryTimeInterval = new TimeSpan(0, 0, 2);
        }

        #endregion

        #region Public Methods

        public List<QueueItemStatusSummary> GetItemStatusSummary()
        {
            if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
            {
                lock (s_thisLock)
                {
                    if (DateTime.Now - s_lastTimeCalled > s_GetItemStatusSummaryTimeInterval)
                    {
                        IQueueItemHeaderDataManager manager = QDataManagerFactory.GetDataManager<IQueueItemHeaderDataManager>();
                        s_itemStatusSummary = manager.GetItemStatusSummary();
                        s_lastTimeCalled = DateTime.Now;
                    }
                }
            }
            return s_itemStatusSummary;

        }


        public IEnumerable<ExecutionFlowStatusSummary> GetExecutionFlowStatusSummary()
        {
            QueueInstanceManager queueInstanceManager = new QueueInstanceManager();
            List<QueueItemStatusSummary> queueItemStatusSummary = GetItemStatusSummary();
            IEnumerable<QueueInstance> queueInstances = queueInstanceManager.GetAllQueueInstances();

            IEnumerable<ExecutionFlowStatusSummary> result = from qInstances in queueInstances
                                                             join qItemStatusSummary in queueItemStatusSummary
                                                             on qInstances.QueueInstanceId equals qItemStatusSummary.QueueId
                                                             group new { qItemStatusSummary, qInstances, qItemStatusSummary.Count }
                                                             by new { qItemStatusSummary, qItemStatusSummary.Status, qInstances.ExecutionFlowId } into res
                                                             select new ExecutionFlowStatusSummary
                                                             {
                                                                 ExecutionFlowId = (Guid)res.Key.ExecutionFlowId,
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

        public Vanrise.Entities.IDataRetrievalResult<QueueItemHeaderDetails> GetFilteredQueueItemHeader(Vanrise.Entities.DataRetrievalInput<QueueItemHeaderQuery> input)
        {

            IQueueItemHeaderDataManager dataManager = QDataManagerFactory.GetDataManager<IQueueItemHeaderDataManager>();

            if (input.Query.QueueIds == null && input.Query.ExecutionFlowIds != null && input.Query.ExecutionFlowIds.Count() > 0)
            {
                QueueInstanceManager queueInstanceManger = new QueueInstanceManager();
                input.Query.QueueIds = queueInstanceManger.GetQueueExecutionFlows(input.Query.ExecutionFlowIds).Select(x => x.QueueInstanceId).ToList();
                input.Query.ExecutionFlowIds = null;
            }


            Vanrise.Entities.BigResult<QueueItemHeader> queueItemHeader = dataManager.GetFilteredQueueItemHeader(input);
            BigResult<QueueItemHeaderDetails> queueItemHeaderDetailResult = new BigResult<QueueItemHeaderDetails>()
            {
                ResultKey = queueItemHeader.ResultKey,
                TotalCount = queueItemHeader.TotalCount,
                Data = queueItemHeader.Data.MapRecords(QueueItemHeaderDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueItemHeaderDetailResult);
        }

        #endregion


        #region Private Methods

        private static TimeSpan s_GetItemStatusSummaryTimeInterval;

        private static DateTime s_lastTimeCalled = new DateTime();

        private static List<QueueItemStatusSummary> s_itemStatusSummary;

        private static Object s_thisLock = new Object();

        private QueueItemHeaderDetails QueueItemHeaderDetailMapper(QueueItemHeader queueItemHeader)
        {
            QueueItemHeaderDetails queueItemHeaderDetail = new QueueItemHeaderDetails();
            QueueInstanceManager queueManager = new QueueInstanceManager();
            QueueExecutionFlowManager executionFlowManager = new QueueExecutionFlowManager();
            var instance = queueManager.GetQueueInstanceById(queueItemHeader.QueueId);
            queueItemHeaderDetail.Entity = queueItemHeader;
            queueItemHeaderDetail.StageName = instance != null ? instance.StageName : "";
            queueItemHeaderDetail.StatusName = Vanrise.Common.Utilities.GetEnumDescription(queueItemHeader.Status);
            queueItemHeaderDetail.QueueTitle = instance.Title;
            queueItemHeaderDetail.ExecutionFlowName = executionFlowManager.GetExecutionFlowName((Guid)instance.ExecutionFlowId);
            return queueItemHeaderDetail;
        }

        #endregion



    }
}
