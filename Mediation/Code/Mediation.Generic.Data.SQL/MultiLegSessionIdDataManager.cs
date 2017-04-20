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
        public MultiLegSessionIdDataManager()
            : base(GetConnectionStringName("Mediation_GenericRecord_DBConnStringKey", "Mediation_GenericRecord_DBConnString"))
        {

        }
        public void DeleteSessionIdFromDB(Guid mediationDefinionId, string sessionId)
        {
            ExecuteNonQuerySP("[Mediation_Generic].[sp_MultiLegSessionID_DeleteSessionId]", mediationDefinionId, sessionId);
        }
        public void AddSessionLegsToDB(Guid mediationDefinitionId, string sessionId, List<string> nonAssociatedLegIds)
        {
            ExecuteNonQuerySP("[Mediation_Generic].[sp_MultiLegSessionID_InsertSessionLegs]", mediationDefinitionId, sessionId, string.Join(",", nonAssociatedLegIds));
        }
        public IEnumerable<MultiLegSessionIdEntity> GetMultiLegSessionIds(Guid mediationDefinitionId)
        {
            return GetItemsSP("[Mediation_Generic].[sp_MultiLegSessionID_GetByMediationDefinitionID]", MultiLegSessionIdEntityMapper, mediationDefinitionId);
        }
        MultiLegSessionIdEntity MultiLegSessionIdEntityMapper(IDataReader reader)
        {
            return new MultiLegSessionIdEntity
            {
                MediationDefinitionId = GetReaderValue<Guid>(reader, "MediationDefinitionID"),
                SessionId = reader["SessionID"] as string,
                LegId = reader["LegId"] as string
            };
        }
    }
}
