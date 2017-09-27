using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Data;

namespace Mediation.Generic.Business
{
    public class MediationCommittedIdManager
    {
        public long? GetLastCommittedId(Guid mediationDefinitionId)
        {
            IMediationCommittedIdDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationCommittedIdDataManager>();
            return dataManager.GetLastCommittedId(mediationDefinitionId);
        }

        public void InsertOrUpdateLastCommitedId(Guid mediationDefinitionId, long lastCommittedId)
        {
            IMediationCommittedIdDataManager dataManager = MediationGenericDataManagerFactory.GetDataManager<IMediationCommittedIdDataManager>();
            dataManager.InsertOrUpdateLastCommitedId(mediationDefinitionId, lastCommittedId);
        }
    }
}
