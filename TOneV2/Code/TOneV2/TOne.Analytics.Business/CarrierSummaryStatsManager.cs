using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business
{
    public class CarrierSummaryStatsManager
    {
        private readonly ICarrierSummaryStatsDataManager _datamanager;

        public CarrierSummaryStatsManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<ICarrierSummaryStatsDataManager>();
        }

        public Vanrise.Entities.IDataRetrievalResult<CarrierSummaryStats> GetFilteredCarrierSummaryStats(Vanrise.Entities.DataRetrievalInput<CarrierSummaryStatsQuery> input)
        {
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, _datamanager.GetCarrierSummaryStatsByCriteria(input));
        }


        //public List<Entities.CarrierSummaryStats> GetCarrierSummaryStats(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, char groupByProfile, int? topCount, string currency)
        //{
        //    return _datamanager.GetCarrierSummaryStats(carrierType, fromDate, toDate, customerID, supplierID, groupByProfile, topCount, currency);
        //}
    }
}
