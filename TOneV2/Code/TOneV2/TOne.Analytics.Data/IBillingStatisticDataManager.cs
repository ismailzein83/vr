using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IBillingStatisticDataManager : IDataManager
    {
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, bool groupByCustomer);

        List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale);
        List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount);
        List<BillingStatistic> GetBillingStatistics(DateTime fromDate,DateTime toDate);
    }
}
