//using Retail.Cost.Entities;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.GenericData.QueueActivators;

//namespace Retail.Cost.Business
//{
//    public class ApplyCostQueueActivator : Vanrise.Queueing.Entities.QueueActivator
//    {

//        #region QueueActivator

//        public override void OnDisposed()
//        {
//        }

//        public override void ProcessItem(Vanrise.Queueing.Entities.PersistentQueueItem item, Vanrise.Queueing.Entities.ItemsToEnqueue outputItems)
//        {
//        }

//        public override void ProcessItem(Vanrise.Queueing.Entities.IQueueActivatorExecutionContext context)
//        {
//            DataRecordBatch dataRecordBatch = context.ItemToProcess as DataRecordBatch;
//            var queueItemType = context.CurrentStage.QueueItemType as DataRecordBatchQueueItemType;
//            if (queueItemType == null)
//                throw new Exception("current stage QueueItemType is not of type DataRecordBatchQueueItemType");
//            var recordTypeId = queueItemType.DataRecordTypeId;
//            var batchRecords = dataRecordBatch.GetBatchRecords(recordTypeId);

//            List<CDRCostRequest> cdrCostRequests = new List<CDRCostRequest>();

//            foreach (var record in batchRecords)
//            {
//                CDRCostRequest cdrCostRequest = new CDRCostRequest()
//                {
//                    OriginalCDR = record,
//                    CDPN = Vanrise.Common.Utilities.GetPropValueReader("CDPN").GetPropertyValue(record),
//                    CGPN = Vanrise.Common.Utilities.GetPropValueReader("CGPN").GetPropertyValue(record),
//                    AttemptDateTime = Vanrise.Common.Utilities.GetPropValueReader("AttemptDateTime").GetPropertyValue(record)
//                };

//                cdrCostRequests.Add(cdrCostRequest);
//            }

//            List<CDRCostResponse> cdrCostResponses = new CDRCostManager().CalculateCost(cdrCostRequests);

//            if (cdrCostResponses != null && cdrCostResponses.Count > 0)
//            {
//                foreach (var cdrCostResponse in cdrCostResponses)
//                {
//                    cdrCostResponse.Request.OriginalCDR.CostAmount = cdrCostResponse.CostAmount;
//                }
//            }
//        }

//        #endregion
//    }
//}