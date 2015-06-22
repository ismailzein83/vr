﻿using System;
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
        List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, int? supplierAMUId, int? customerAMUId);
        List<ZoneSummary> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier);
        List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale);
        List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount);
        List<VariationReports> GetVariationReportsData(List<TimeRange> timeRange, VariationReportOptions variationReportOptions);
        List<ZoneSummaryDetailed> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier);
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
        List<CustomerRouting> GetCustomerRouting(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
    }
}
