using Mediation.Generic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            foreach(var legId in legIds)
            {
                bool isLegAssociatedToSession = false;
                if (sessionId == null)//sessionId not found yet, try to locate it
                {
                    if(sessionIdsMemStore.SessionIdsByLegId.TryGetValue(legId, out sessionId))
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
            if(sessionId == null)
            {
                sessionId = Guid.NewGuid().ToString();
                sessionLegIds = new HashSet<string>();
                sessionIdsMemStore.LegIdsBySessionId.Add(sessionId, sessionLegIds);
            }
            if(nonAssociatedLegIds.Count > 0)
            {
                AddSessionLegsToDB(sessionId, nonAssociatedLegIds);
                foreach(var legId in nonAssociatedLegIds)
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
            throw new NotImplementedException();
        }

        private void AddSessionLegsToDB(string sessionId, List<string> nonAssociatedLegIds)
        {
            throw new NotImplementedException();
        }

        private void DeleteSessionIdFromDB(string sessionId)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    internal class MultiLegSessionIdsMemoryStore
    {
        public Dictionary<string,string> SessionIdsByLegId { get; set; }

        public Dictionary<string, HashSet<string>> LegIdsBySessionId { get; set; }
    }
}
