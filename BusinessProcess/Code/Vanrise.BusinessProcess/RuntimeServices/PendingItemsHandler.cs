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

        Dictionary<Guid, ConcurrentQueue<Guid>> _qDefinitionsHavingPendingInstances = new Dictionary<Guid, ConcurrentQueue<Guid>>();

        Dictionary<Guid, ConcurrentQueue<BPInstanceCancellationRequest>> _qCancellatioRequestsByServiceInstanceId = new Dictionary<Guid, ConcurrentQueue<BPInstanceCancellationRequest>>();

        internal void SetPendingDefinitionsToProcess(Guid serviceInstanceId, List<Guid> bpDefinitionIds)
        {
            ConcurrentQueue<Guid> qPendingBPDefinitionIds;
            List<Guid> existingPendingDefinitionIds;
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

        internal bool TryGetPendingDefinitionsToProcess(Guid serviceInstanceId, out Guid bpDefinitionId)
        {
            ConcurrentQueue<Guid> qPendingBPDefinitionIds = null;
            lock (_qDefinitionsHavingPendingInstances)
            {
                qPendingBPDefinitionIds = _qDefinitionsHavingPendingInstances.GetOrCreateItem(serviceInstanceId);
            }
            return qPendingBPDefinitionIds.TryDequeue(out bpDefinitionId);
        }

        internal void AddCancellationRequest(Guid serviceInstanceId, BPInstanceCancellationRequest cancellationRequest)
        {
            ConcurrentQueue<BPInstanceCancellationRequest> qCancellationRequests;
            List<BPInstanceCancellationRequest> existingCancellationRequests;
            lock (_qCancellatioRequestsByServiceInstanceId)
            {
                qCancellationRequests = _qCancellatioRequestsByServiceInstanceId.GetOrCreateItem(serviceInstanceId);
                existingCancellationRequests = qCancellationRequests.ToList();
            }
            if (!existingCancellationRequests.Any(itm => itm.BPInstanceId == cancellationRequest.BPInstanceId))
                qCancellationRequests.Enqueue(cancellationRequest);
        }

        internal bool TryGetCancellationRequestToProcess(Guid serviceInstanceId, out BPInstanceCancellationRequest cancellationRequest)
        {
            ConcurrentQueue<BPInstanceCancellationRequest> qCancellationRequests;
            lock (_qCancellatioRequestsByServiceInstanceId)
            {
                qCancellationRequests = _qCancellatioRequestsByServiceInstanceId.GetOrCreateItem(serviceInstanceId);
            }
            return qCancellationRequests.TryDequeue(out cancellationRequest);
        }
    }

    internal class BPInstanceCancellationRequest
    {
        public Guid BPDefinitionId { get; set; }

        public long BPInstanceId { get; set; }

        public string Reason { get; set; }
    }
}
