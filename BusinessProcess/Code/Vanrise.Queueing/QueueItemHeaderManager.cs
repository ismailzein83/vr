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
    public class QueueItemHeaderManager
    {

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


        private QueueItemHeaderDetails QueueItemHeaderDetailMapper(QueueItemHeader queueItemHeader)
        {
            QueueItemHeaderDetails queueItemHeaderDetail = new QueueItemHeaderDetails();
            QueueingManager manager = new QueueingManager();
            QueueExecutionFlowManager executionFlowManager=new QueueExecutionFlowManager();
            var instance = manager.GetQueueInstanceById(queueItemHeader.QueueId);
            queueItemHeaderDetail.Entity = queueItemHeader;
            queueItemHeaderDetail.StageName = instance != null ? instance.StageName : "";
            queueItemHeaderDetail.StatusName = Vanrise.Common.Utilities.GetEnumDescription(queueItemHeader.Status);
            queueItemHeaderDetail.QueueTitle = instance.Title;
            queueItemHeaderDetail.ExecutionFlowName = executionFlowManager.GetExecutionFlowName((int)instance.ExecutionFlowId);
            return queueItemHeaderDetail;
        }

    }
}
