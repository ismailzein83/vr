using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    internal class PendingItemsHandler
    {
        #region Singleton

        static PendingItemsHandler _current = new PendingItemsHandler();
        internal static PendingItemsHandler Current
        {
            get
            {
                return _current;
            }
        }

        private PendingItemsHandler()
        {

        }

        #endregion

        Dictionary<Guid, ConcurrentQueue<int>> _qPendingQueueIdsByServiceInstanceId = new Dictionary<Guid, ConcurrentQueue<int>>();

        public void SetPendingQueuesToProcess(Guid serviceInstanceId, List<int> queueIds)
        {
            ConcurrentQueue<int> qPendingQueueIds;
            List<int> existingPendingQueueIds;
            lock (_qPendingQueueIdsByServiceInstanceId)
            {
                qPendingQueueIds = _qPendingQueueIdsByServiceInstanceId.GetOrCreateItem(serviceInstanceId);
                existingPendingQueueIds = qPendingQueueIds.ToList();
            }

            foreach (var queueId in queueIds)
            {
                if (!existingPendingQueueIds.Contains(queueId))
                    qPendingQueueIds.Enqueue(queueId);
            }
        }

        internal bool TryGetPendingQueueToProcess(Guid serviceInstanceId, out int queueId)
        {
            ConcurrentQueue<int> qPendingQueueIds = null;
            lock (_qPendingQueueIdsByServiceInstanceId)
            {
                qPendingQueueIds = _qPendingQueueIdsByServiceInstanceId.GetOrCreateItem(serviceInstanceId);
            }
            return qPendingQueueIds.TryDequeue(out queueId);
        }

        HashSet<Guid> _pendingSummaryItemsToProcessServiceInstanceIds = new HashSet<Guid>();

        public void SetPendingSummaryItemsToProcess(Guid serviceInstanceId)
        {
            lock (_pendingSummaryItemsToProcessServiceInstanceIds)
            {
                _pendingSummaryItemsToProcessServiceInstanceIds.Add(serviceInstanceId);
            }
        }

        internal bool HasSummaryItemsToProcess(Guid serviceInstanceId)
        {
            bool hasSummaryItemsToProcess = false;
            lock (_pendingSummaryItemsToProcessServiceInstanceIds)
            {
                if(_pendingSummaryItemsToProcessServiceInstanceIds.Contains(serviceInstanceId))
                {
                    hasSummaryItemsToProcess = true;
                    _pendingSummaryItemsToProcessServiceInstanceIds.Remove(serviceInstanceId);
                }
            }
            return hasSummaryItemsToProcess;
        }

    }
}
