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
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, int? supplierAMUId, int? customerAMUId);
        List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier);
        List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale);
        List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount);
        List<BillingStatistic> GetBillingStatistics(DateTime fromDate,DateTime toDate);
        List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue);
        List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier);
        List<CarrierLost> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId);
        List<CarrierSummaryDaily> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId);
    }
}
