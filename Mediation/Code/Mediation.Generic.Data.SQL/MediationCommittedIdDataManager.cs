using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Mediation.Generic.Data.SQL
{
    public class MediationCommittedIdDataManager : BaseSQLDataManager, IMediationCommittedIdDataManager
    {
        public MediationCommittedIdDataManager()
            : base(GetConnectionStringName("Mediation_GenericRecord_DBConnStringKey", "Mediation_GenericRecord_DBConnString"))
        {

        }

        public long? GetLastCommittedId(Guid mediationDefinitionId)
        {
            var id = ExecuteScalarSP("[Mediation_Generic].[sp_MediationLastCommittedId_Get]", mediationDefinitionId);
            return (long?)id;
        }
        public bool InsertOrUpdateLastCommitedId(Guid mediationDefinitionId, long lastCommittedId)
        {
            return ExecuteNonQuerySP("[Mediation_Generic].[sp_MediationLastCommittedId_InsertOrUpdate]", mediationDefinitionId, lastCommittedId) > 0;
        }
    }
}
