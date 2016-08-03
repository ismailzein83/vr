using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Queueing
{
    public class QueueActivationRuntimeWCFService : IQueueActivationRuntimeWCFService
    {
        static Dictionary<Guid, ConcurrentQueue<int>> s_qPendingQueueIdsByActivator = new Dictionary<Guid, ConcurrentQueue<int>>();

        public void SetPendingQueuesToProcess(Guid activatorId, List<int> queueIds)
        {
            ConcurrentQueue<int> qPendingQueueIds;
            List<int> existingPendingQueueIds;
            lock (s_qPendingQueueIdsByActivator)
            {
                qPendingQueueIds = s_qPendingQueueIdsByActivator.GetOrCreateItem(activatorId);
                existingPendingQueueIds = qPendingQueueIds.ToList();
            }

            foreach(var queueId in queueIds)
            {
                if (!existingPendingQueueIds.Contains(queueId))
                    qPendingQueueIds.Enqueue(queueId);
            }
        }

        internal static bool TryGetPendingQueueToProcess(Guid activatorId, out int queueId)
        {
            ConcurrentQueue<int> qPendingQueueIds = null;
            lock (s_qPendingQueueIdsByActivator)
            {
               qPendingQueueIds = s_qPendingQueueIdsByActivator.GetOrCreateItem(activatorId);
            }
            return qPendingQueueIds.TryDequeue(out queueId);
        }
    }
}
