using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mediation.Generic.Data
{
    public interface IMediationCommittedIdDataManager : IDataManager
    {
        long? GetLastCommittedId(Guid mediationDefinitionId);

        bool InsertOrUpdateLastCommitedId(Guid mediationDefinitionId, long lastCommittedId);
    }
}
