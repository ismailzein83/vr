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
        List<CarrierProfileReport> GetCarrierProfileMTDAndMTA(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale);
        List<CarrierProfileReport> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int topDestination, bool isSale, bool isAmount);
        List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, List<string> customersIds, List<string> suppliersIds, bool groupBySupplier, out double services);
        List<DailySummary> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<CarrierLost> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId);
        List<CarrierSummaryDaily> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId);
        List<RateLoss> GetRateLoss(DateTime fromDate, DateTime toDate,string customerID, string supplier ,int? zoneId , int? customerAMUId, int? supplierAMUId);
        List<CarrierSummary> GetCarrierSummary(DateTime fromDate, DateTime toDate,string customerID, string supplier , int? customerAMUId, int? supplierAMUId);
        List<CustomerSummary> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, int? customerAMUId, int? supplierAMUId);
        List<CustomerServices> GetCustomerServices(DateTime fromDate, DateTime toDate);
        List<DetailedCarrierSummary> GetCarrierDetailedSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId);
        List<DailyForcasting> GetDailyForcasting(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<ExchangeCarriers> GetExchangeCarriers(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<CustomerRouting> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId);
        List<RoutingAnalysis> GetRoutingAnalysis(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? top, int? customerAMUId, int? supplierAMUId);
        List<SupplierCostDetails> GetSupplierCostDetails(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<SaleZoneCostSummary> GetSaleZoneCostSummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<SaleZoneCostSummaryService> GetSaleZoneCostSummaryService(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);
        List<SaleZoneCostSummarySupplier> GetSaleZoneCostSummarySupplier(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId);

        List<VariationReports> GetVariationReportsData(List<TimeRange> timeRange, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy, out int totalCount, out  List<decimal> totalValues, out List<DateTime> datetotalValues, out decimal TotalAverage);
        List<VolumeTraffic> GetTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, string zoneID, int attempts, VolumeReportsTimePeriod timePeriod);
        DestinationVolumeTrafficResult GetDestinationTrafficVolumes(DateTime fromDate, DateTime toDate, string customerID, string supplierID, int zoneID, int attempts, VolumeReportsTimePeriod timePeriod, int topDestination, List<TimeRange> timeRange, bool isDuration);

        List<InOutVolumeTraffic> CompareInOutTraffic(DateTime fromDate, DateTime toDate, string customerId, VolumeReportsTimePeriod timePeriod);
        
    }
}
