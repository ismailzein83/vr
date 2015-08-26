using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface ICarrierSummaryStatsDataManager : IDataManager
    {
        //Vanrise.Entities.BigResult<Entities.CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<Entities.CarrierSummaryStatsQuery> input);
        CarrierSummaryBigResult<CarrierSummaryStats> GetCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input);
    }
}
