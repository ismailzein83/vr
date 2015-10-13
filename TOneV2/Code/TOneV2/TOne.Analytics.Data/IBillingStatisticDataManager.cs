using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Data
{
    public interface IBillingStatisticDataManager : IDataManager
    {
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, List<string> supplierIds, List<string> customerIds, string currencyId);
        List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, List<string> customersIds, List<string> suppliersIds, bool groupBySupplier, out double services);
        List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale);
        List<CarrierProfileReport> GetCarrierProfileMTDAndMTA(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale , string CurrencyId);
        List<CarrierProfileReport> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int topDestination, bool isSale, bool isAmount , string CurrencyId);
        List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, List<string> customersIds, List<string> suppliersIds, bool groupBySupplier, out double services);
        List<DailySummary> GetDailySummary(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string currencyId);
        List<CarrierLost> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, string currencyId, List<string> customerIds, List<string> supplierIds);
        List<CarrierSummaryDaily> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, List<string> customersIds, List<string> suppliersIds, string currencyId);
        List<RateLoss> GetRateLoss(DateTime fromDate, DateTime toDate, string customerID, string supplier, string zonesId, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<CarrierSummary> GetCarrierSummary(DateTime fromDate, DateTime toDate, string customerID, string supplier, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<CustomerSummary> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, List<string> customerIds, List<string> supplierIds, string currencyId);
        List<CustomerServices> GetCustomerServices(DateTime fromDate, DateTime toDate);
        List<DetailedCarrierSummary> GetCarrierDetailedSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<DailyForcasting> GetDailyForcasting(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<ExchangeCarriers> GetExchangeCarriers(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<CustomerRouting> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, List<string> customerIds, List<string> supplierIds, string currencyId);
        List<RoutingAnalysis> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, List<string> supplierIds, List<string> customerIds, string currencyId);
        List<SupplierCostDetails> GetSupplierCostDetails(DateTime fromDate, DateTime toDate,  List<string> customerIds,List<string> supplierIds , string CurrencyId);
        List<SaleZoneCostSummary> GetSaleZoneCostSummary(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId);
        List<SaleZoneCostSummaryService> GetSaleZoneCostSummaryService(DateTime fromDate, DateTime toDate,   List<string> customerIds,List<string> supplierIds , string CurrencyId);
        List<SaleZoneCostSummarySupplier> GetSaleZoneCostSummarySupplier(DateTime fromDate, DateTime toDate, List<string> customerIds, List<string> supplierIds, string CurrencyId);

        List<VariationReports> GetVariationReportsData(List<TimeRange> timeRange, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy, out int totalCount, out  List<decimal> totalValues, out List<DateTime> datetotalValues, out decimal TotalAverage);
        List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, string zoneID, int attempts, VolumeReportsTimePeriod timePeriod);
        DestinationVolumeTrafficResult GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, int zoneID, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination, List<TimeRange> timeRange, bool isDuration);

        List<InOutVolumeTraffic> CompareInOutTraffic(DateTime fromDate, DateTime toDate, string customerId, VolumeReportsTimePeriod timePeriod);
        
    }
}
