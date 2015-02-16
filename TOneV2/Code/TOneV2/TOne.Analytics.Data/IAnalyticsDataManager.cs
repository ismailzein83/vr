using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IAnalyticsDataManager : IDataManager
    {
        List<TopNDestinationView> GetTopNDestinations(int topCount,
           DateTime fromDate,
           DateTime toDate,
           string sortOrder,
           string customerID,
           string supplierID,
           int? switchID,
           char groupByCodeGroup,
           string codeGroup,
           char showSupplier,
            string orderTarget,
            int from,
            int to);

        List<Alert> GetAlerts(int topCount, char showHiddenAlerts, int alertLevel, string tag, string source, int? userID);

        List<CarrierRateView> GetRates(string carrierType, DateTime effectiveOn, string carrierID, string codeGroup, int from, int to);

        List<Entities.CarrierSummaryView> GetCarrierSummary(string carrierType, DateTime fromDate, DateTime toDate, string customerID, string supplierID, int topCount, char groupByProfile);

        List<Entities.TopCarriersView> GetTopCustomers(DateTime fromDate, DateTime toDate, int topCount);

        List<Entities.TopCarriersView> GetTopSupplier(DateTime fromDate, DateTime toDate, int topCount);

        List<Entities.ProfitByDay> GetLastWeeksProfit(DateTime from, DateTime to);

        List<Entities.TrafficSummaryView> GetSummary(DateTime fromDate, DateTime toDate);
    }
}
