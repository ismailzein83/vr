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
            Vanrise.Entities.BigResult<QueueItemHeader> queueItemHeader = dataManager.GetQueueItemHeaderFilteredFromTemp(input);
            BigResult<QueueItemHeaderDetails> queueItemHeaderDetailResult = new BigResult<QueueItemHeaderDetails>()
            {
                ResultKey=queueItemHeader.ResultKey,
                TotalCount=queueItemHeader.TotalCount,
                Data=queueItemHeader.Data.MapRecords(QueueItemHeaderDetailMapper)
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, queueItemHeaderDetailResult);
        }


        private QueueItemHeaderDetails QueueItemHeaderDetailMapper(QueueItemHeader queueItemHeader)
        {
            QueueItemHeaderDetails queueItemHeaderDetail = new QueueItemHeaderDetails();
            queueItemHeaderDetail.Entity = queueItemHeader;
            queueItemHeaderDetail.StageName = queueItemHeader.Description;
            return queueItemHeaderDetail;
        }

    }
}
