using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Data
{
    public interface ICarrierSummaryStatsDataManager : IDataManager
    {
        //List<Entities.CarrierSummaryStats> GetCarrierSummaryStats(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, char groupByProfile, int? topCount, string currency);

        Vanrise.Entities.BigResult<Entities.CarrierSummaryStats> GetCarrierSummaryStatsByCriteria(Vanrise.Entities.DataRetrievalInput<Entities.CarrierSummaryStatsQuery> input);

    }
}
