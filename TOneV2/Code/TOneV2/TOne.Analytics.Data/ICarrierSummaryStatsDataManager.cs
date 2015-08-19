using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Data
{
    public interface ICarrierSummaryStatsDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<Entities.CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<Entities.CarrierSummaryStatsQuery> input);
    }
}
