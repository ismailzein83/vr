using System;
using System.Collections.Generic;
using Vanrise.Data.SQL;
using Vanrise.Common;
using System.Linq;
using Mediation.Generic.Entities;
using System.Data;

namespace Mediation.Generic.Data.SQL
{
    public class MultiLegSessionIdDataManager : BaseSQLDataManager, IMultiLegSessionIdDataManager
    {
        public void DeleteSessionIdFromDB(int mediationDefinionId, string sessionId)
        {
            ExecuteNonQuerySP("[Mediation_Generic].[sp_MultiLegSessionID_DeleteSessionId]", mediationDefinionId, sessionId);
        }
        public void AddSessionLegsToDB(int mediationDefinitionId, string sessionId, List<string> nonAssociatedLegIds)
        {
            ExecuteNonQuerySP("[Mediation_Generic].[sp_MultiLegSessionID_InsertSessionLegs]", mediationDefinitionId, sessionId, string.Join(",", nonAssociatedLegIds));
        }
        public IEnumerable<MultiLegSessionIdEntity> GetMultiLegSessionIds(int mediationDefinitionId)
        {
            return GetItemsSP("[Mediation_Generic].[sp_MultiLegSessionID_GetByMediationDefinitionID]", MultiLegSessionIdEntityMapper, mediationDefinitionId);
        }
        MultiLegSessionIdEntity MultiLegSessionIdEntityMapper(IDataReader reader)
        {
            return new MultiLegSessionIdEntity
            {
                MediationDefinitionId = (int)reader["MediationDefinitionID"],
                SessionId = reader["SessionID"] as string,
                LegId = reader["LegId"] as string
            };
        }
    }
}
