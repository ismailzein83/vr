using System;
using System.Collections.Generic;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICodeMatchesDataManager : IDataManager, IBulkApplyDataManager<CodeMatches>, IRoutingDataManager
    {
        bool ShouldApplyCodeZoneMatch { get; set; }

        void ApplyCodeMatchesForDB(object preparedCodeMatches);

        Dictionary<long, RPCodeMatches> GetRPCodeMatchesBySaleZone(long fromZoneId, long toZoneId, Func<bool> shouldStop);

        List<PartialCodeMatches> GetPartialCodeMatchesByRouteCodes(HashSet<string> routeCodes);
    }
}
