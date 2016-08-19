using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.BusinessProcess
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

        Dictionary<Guid, ConcurrentQueue<int>> _qDefinitionsHavingPendingInstances = new Dictionary<Guid, ConcurrentQueue<int>>();

        internal void SetPendingDefinitionsToProcess(Guid serviceInstanceId, List<int> bpDefinitionIds)
        {
            ConcurrentQueue<int> qPendingBPDefinitionIds;
            List<int> existingPendingDefinitionIds;
            lock (_qDefinitionsHavingPendingInstances)
            {
                qPendingBPDefinitionIds = _qDefinitionsHavingPendingInstances.GetOrCreateItem(serviceInstanceId);
                existingPendingDefinitionIds = qPendingBPDefinitionIds.ToList();
            }

            foreach (var definitionId in bpDefinitionIds)
            {
                if (!existingPendingDefinitionIds.Contains(definitionId))
                    qPendingBPDefinitionIds.Enqueue(definitionId);
            }
        }

        internal bool TryGetPendingDefinitionsToProcess(Guid serviceInstanceId, out int bpDefinitionId)
        {
            ConcurrentQueue<int> qPendingBPDefinitionIds = null;
            lock (_qDefinitionsHavingPendingInstances)
            {
                qPendingBPDefinitionIds = _qDefinitionsHavingPendingInstances.GetOrCreateItem(serviceInstanceId);
            }
            return qPendingBPDefinitionIds.TryDequeue(out bpDefinitionId);
        }
    }
}
