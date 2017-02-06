using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Data;

namespace Mediation.Generic.Business
{
    public class MediationProcessContext : IMediationProcessContext
    {
        int _mediationDefinitionId;

        public MediationProcessContext(int mediationDefinitionId)
        {
            _mediationDefinitionId = mediationDefinitionId;
        }

        #region Public Methods

        public string GetMultiLegSessionId(IEnumerable<string> legIds)
        {
            MultiLegSessionIdsMemoryStore sessionIdsMemStore = GetSessionIdsMemStore();
            string sessionId = null;
            List<string> nonAssociatedLegIds = new List<string>();
            HashSet<string> sessionLegIds = null;
            foreach (var legId in legIds)
            {
                bool isLegAssociatedToSession = false;
                if (sessionId == null)//sessionId not found yet, try to locate it
                {
                    if (sessionIdsMemStore.SessionIdsByLegId.TryGetValue(legId, out sessionId))
                    {
                        sessionLegIds = sessionIdsMemStore.LegIdsBySessionId[sessionId];
                        isLegAssociatedToSession = true;
                    }
                }
                else//sessionId found in previous iteration
                {
                    isLegAssociatedToSession = sessionLegIds.Contains(legId);
                }
                if (!isLegAssociatedToSession)
                {
                    nonAssociatedLegIds.Add(legId);
                }
            }
            if (sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                sessionLegIds = new HashSet<string>();
                sessionIdsMemStore.LegIdsBySessionId.Add(sessionId, sessionLegIds);
            }
            if (nonAssociatedLegIds.Count > 0)
            {
                AddSessionLegsToDB(sessionId, nonAssociatedLegIds);
                foreach (var legId in nonAssociatedLegIds)
                {
                    sessionLegIds.Add(legId);
                    sessionIdsMemStore.SessionIdsByLegId.Add(legId, sessionId);
                }
            }
            return sessionId;
        }

        public void DeleteSessionId(string sessionId)
        {
            MultiLegSessionIdsMemoryStore sessionIdsMemStore = GetSessionIdsMemStore();
            DeleteSessionIdFromDB(sessionId);
            HashSet<string> sessionLegIds;
            if (sessionIdsMemStore.LegIdsBySessionId.TryGetValue(sessionId, out sessionLegIds))
            {
                sessionIdsMemStore.LegIdsBySessionId.Remove(sessionId);
                foreach (var legId in sessionLegIds)
                {
                    sessionIdsMemStore.SessionIdsByLegId.Remove(legId);
                }
            }
        }

        #endregion

        #region Private Methods

        private MultiLegSessionIdsMemoryStore GetSessionIdsMemStore()
        {
            MultiLegSessionIdsMemoryStore store = new MultiLegSessionIdsMemoryStore();
            IMultiLegSessionIdDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMultiLegSessionIdDataManager>();
            IEnumerable<MultiLegSessionIdEntity> multiLegSessionIdEntities = dataManager.GetMultiLegSessionIds(_mediationDefinitionId);

            foreach (var item in multiLegSessionIdEntities)
            {
                HashSet<string> legIds;
                if (!store.LegIdsBySessionId.TryGetValue(item.SessionId, out legIds))
                {
                    legIds = new HashSet<string>();
                    store.LegIdsBySessionId.Add(item.SessionId, legIds);
                }
                legIds.Add(item.LegId);

                if (!store.SessionIdsByLegId.ContainsKey(item.LegId))
                    store.SessionIdsByLegId.Add(item.LegId, item.SessionId);
            }

            return store;
        }

        private void AddSessionLegsToDB(string sessionId, List<string> nonAssociatedLegIds)
        {
            if (nonAssociatedLegIds == null)
                return;
            IMultiLegSessionIdDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMultiLegSessionIdDataManager>();
            dataManager.AddSessionLegsToDB(_mediationDefinitionId, sessionId, nonAssociatedLegIds);
        }

        private void DeleteSessionIdFromDB(string sessionId)
        {
            IMultiLegSessionIdDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMultiLegSessionIdDataManager>();
            dataManager.DeleteSessionIdFromDB(_mediationDefinitionId, sessionId);
        }

        #endregion
    }

    internal class MultiLegSessionIdsMemoryStore
    {
        public MultiLegSessionIdsMemoryStore()
        {
            this.SessionIdsByLegId = new Dictionary<string, string>();
            this.LegIdsBySessionId = new Dictionary<string, HashSet<string>>();
        }
        public Dictionary<string, string> SessionIdsByLegId { get; set; }

        public Dictionary<string, HashSet<string>> LegIdsBySessionId { get; set; }
    }
}
