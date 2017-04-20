using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mediation.Generic.Entities;

namespace Mediation.Generic.Data
{
    public interface IMultiLegSessionIdDataManager : IDataManager
    {
        IEnumerable<MultiLegSessionIdEntity> GetMultiLegSessionIds(Guid mediationDefinitionId);
        void DeleteSessionIdFromDB(Guid mediationDefinionId, string sessionId);
        void AddSessionLegsToDB(Guid mediationDefinitionId, string sessionId, List<string> nonAssociatedLegIds);
    }
}
