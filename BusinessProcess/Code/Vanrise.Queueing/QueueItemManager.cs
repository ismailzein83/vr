using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Queueing.Data;
using Vanrise.Queueing.Entities;

namespace Vanrise.Queueing
{
    public class QueueItemManager
    {
        public List<PendingQueueItemInfo> GetPendingQueueItems(int maxNbOfPendingItems, IOrderedEnumerable<HoldRequest> holdRequests)
        {
            List<PendingQueueItemInfo> pendingQueueItemInfos = new List<PendingQueueItemInfo>();
            IQueueItemDataManager queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();

            int maxNbOfPendingItemsToLoad = holdRequests == null || holdRequests.Count() == 0 ? maxNbOfPendingItems : Int32.MaxValue;

            queueItemDataManager.GetPendingQueueItems(maxNbOfPendingItemsToLoad, (pendingQueueItemInfo) =>
            {
                if (holdRequests == null || holdRequests.Count() == 0)
                    return AddQueueItemWithCountValidation(pendingQueueItemInfos, pendingQueueItemInfo, maxNbOfPendingItems);

                IEnumerable<HoldRequest> filteredHoldRequests = holdRequests.FindAllRecords(itm => itm.QueuesToHold.Contains(pendingQueueItemInfo.QueueId));
                if (filteredHoldRequests == null || filteredHoldRequests.Count() == 0)
                    return AddQueueItemWithCountValidation(pendingQueueItemInfos, pendingQueueItemInfo, maxNbOfPendingItems);

                bool isOverlapped = false;
                foreach (HoldRequest holdRequest in filteredHoldRequests)
                {
                    if (holdRequest.IsOverlappedWith(pendingQueueItemInfo))
                    {
                        isOverlapped = true;
                        break;
                    }
                }

                if (!isOverlapped)
                    return AddQueueItemWithCountValidation(pendingQueueItemInfos, pendingQueueItemInfo, maxNbOfPendingItems);

                return false;
            });

            return pendingQueueItemInfos.Count > 0 ? pendingQueueItemInfos : null;
        }

        private bool AddQueueItemWithCountValidation(List<PendingQueueItemInfo> pendingQueueItemInfos, PendingQueueItemInfo pendingQueueItemInfo, int maxNbOfPendingItems)
        {
            pendingQueueItemInfos.Add(pendingQueueItemInfo);
            if (pendingQueueItemInfos.Count == maxNbOfPendingItems)
                return true;
            else
                return false;
        }

        public List<SummaryBatch> GetSummaryBatches(IOrderedEnumerable<HoldRequest> holdRequests)
        {
            IQueueItemDataManager queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            List<SummaryBatch> summaryBatches = queueItemDataManager.GetSummaryBatches();

            if (holdRequests == null || holdRequests.Count() == 0)
                return summaryBatches;

            List<SummaryBatch> filteredSummaryBatches = new List<SummaryBatch>();
            foreach (SummaryBatch summaryBatch in summaryBatches)
            {
                bool mustBeAdded = true;

                foreach (HoldRequest holdRequest in holdRequests)
                {
                    if (holdRequest.QueuesToHold.Contains(summaryBatch.QueueId) && holdRequest.IsOverlappedWith(summaryBatch))
                    {
                        mustBeAdded = false;
                        break;
                    }
                }
                if (mustBeAdded)
                    filteredSummaryBatches.Add(summaryBatch);
            }

            return filteredSummaryBatches.Count > 0 ? filteredSummaryBatches : null;
        }

        public bool HasPendingQueueItems(List<int> queueIdsToCheck, DateTime from, DateTime to, bool onlyAssignedQueues)
        {
            IQueueItemDataManager queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            return queueItemDataManager.HasPendingQueueItems(queueIdsToCheck, from, to, onlyAssignedQueues);
        }

        public bool HasPendingSummaryQueueItems(List<int> queueIdsToCheck, DateTime from, DateTime to)
        {
            IQueueItemDataManager queueItemDataManager = QDataManagerFactory.GetDataManager<IQueueItemDataManager>();
            return queueItemDataManager.HasPendingSummaryQueueItems(queueIdsToCheck, from, to);
        }
    }
}