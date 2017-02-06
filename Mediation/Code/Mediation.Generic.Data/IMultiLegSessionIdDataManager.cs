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
        IEnumerable<MultiLegSessionIdEntity> GetMultiLegSessionIds(int mediationDefinitionId);
        void DeleteSessionIdFromDB(int mediationDefinionId, string sessionId);
        void AddSessionLegsToDB(int mediationDefinitionId, string sessionId, List<string> nonAssociatedLegIds);
    }
}
