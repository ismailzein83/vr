using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using Vanrise.Data;

namespace TOne.WhS.Routing.Data
{
    public interface ICodeMatchesDataManager : IDataManager, IBulkApplyDataManager<CodeMatches>, IRoutingDataManager
    {
		bool ShouldApplyCodeZoneMatch { get; set; }

        void ApplyCodeMatchesForDB(object preparedCodeMatches);

        IEnumerable<RPCodeMatches> GetRPCodeMatches(long fromZoneId, long toZoneId, Func<bool> shouldStop);

        List<PartialCodeMatches> GetPartialCodeMatchesByRouteCodes(HashSet<string> routeCodes);
    }
}
