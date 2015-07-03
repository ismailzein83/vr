using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;

namespace TOne.Analytics.Business
{
    public partial class BillingStatisticManager
    {
        private readonly IBillingStatisticDataManager _datamanager;
        private readonly BusinessEntityInfoManager _bemanager;
        private readonly CarrierManager _cmanager;

        public BillingStatisticManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            _bemanager = new BusinessEntityInfoManager();
            _cmanager = new CarrierManager();
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, bool groupByCustomer)
        {

            return GetZoneProfit(fromDate, toDate, null, null, groupByCustomer, null, null);
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool groupByCustomer, int? supplierAMUId, int? customerAMUId)
        {

            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, customerId, supplierId, groupByCustomer, supplierAMUId, customerAMUId));
        }

        public List<DailySummaryFormatted> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatDailySummaries(_datamanager.GetDailySummary(fromDate, toDate, customerAMUId, supplierAMUId));
        }

        public List<ZoneSummaryFormatted> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return FormatZoneSummaries(_datamanager.GetZoneSummary(fromDate, toDate, customerId, supplierId, isCost, currencyId, supplierGroup, customerGroup, customerAMUId, supplierAMUId, groupBySupplier));
        }
        public List<ZoneSummaryDetailedFormatted> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return FormatZoneSummariesDetailed(_datamanager.GetZoneSummaryDetailed(fromDate, toDate, customerId, supplierId, isCost, currencyId, supplierGroup, customerGroup, customerAMUId, supplierAMUId, groupBySupplier));
        }

        public List<CarrierLostFormatted> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId)
        {
            return FormatCarrierLost(_datamanager.GetCarrierLost(fromDate, toDate, customerId, supplierId, margin, supplierAMUId, customerAMUId));
        }
        public List<MonthTraffic> GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {

            return _datamanager.GetMonthTraffic(fromDate, toDate, carrierAccountID, isSale);
        }

        public List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount)
        {
            return _datamanager.GetCarrierProfile(fromDate, toDate, carrierAccountID, TopDestinations, isSale, IsAmount);
        }

        public List<CarrierSummaryDailyFormatted> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId)
        {
            return FormatCarrieresSummaryDaily(_datamanager.GetDailyCarrierSummary(fromDate, toDate, customerId, supplierId, isCost, isGroupedByDay, supplierAMUId, customerAMUId), isGroupedByDay);
        }

        public VariationReportResult GetVariationReportsData(DateTime selectedDate, int periodCount, TimePeriod timePeriod, VariationReportOptions variationReportOptions, int fromRow, int toRow, EntityType entityType, string entityID, GroupingBy groupingBy)
        {
            List<TimeRange> timeRanges = new List<TimeRange>();
            DateTime currentDate = new DateTime();
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            currentDate = selectedDate.AddDays(1);
            int counter = periodCount;
            while (counter > 0)
            {
                switch (timePeriod)
                {
                    case TimePeriod.Days:
                        fromDate = currentDate.AddDays(-1);
                        toDate = currentDate;
                        break;
                    case TimePeriod.Weeks:
                        fromDate = currentDate.AddDays(-7);
                        toDate = currentDate;
                        break;
                    case TimePeriod.Months:
                        fromDate = currentDate.AddMonths(-1);
                        toDate = currentDate;
                        break;
                }
                TimeRange timeRange = new TimeRange { FromDate = fromDate, ToDate = toDate };
                timeRanges.Add(timeRange);
                currentDate = fromDate;
                counter = counter - 1;

            }
            int totalCount;
            List<decimal> totalValues,totals;
            List<DateTime> datetotalValues;
            decimal totalAverage,totalPercentage,totalPreviousPercentage;
            List<VariationReports> variationReports = _datamanager.GetVariationReportsData(timeRanges, variationReportOptions, fromRow, toRow,entityType,entityID,groupingBy, out totalCount, out totalValues, out datetotalValues, out totalAverage);
            var result = GetVariationReportsData(variationReports, timeRanges, selectedDate, periodCount, totalValues, datetotalValues, totalAverage, out totals,out totalPercentage,out totalPreviousPercentage);
            result.TotalCount = totalCount;
            result.TotalValues = totals;
            result.TotalAverage = totalAverage;
            result.TotalPercentage = totalPercentage;
            result.TotalPreviousPercentage = totalPreviousPercentage;
            return result;
        }

        public VariationReportResult GetVariationReportsData(List<VariationReports> variationReports, List<TimeRange> timeRanges, DateTime selectedDate, int periodCount, List<decimal> totalValues, List<DateTime> datetotalValues, decimal totalAverage, out List<decimal> totals, out decimal totalPercentage, out decimal totalPreviousPercentage)
        {
            List<VariationReportsData> variationReportsData = new List<VariationReportsData>();
            VariationReportsData current = null;
            foreach (var item in variationReports)
            {
                if (current == null || current.ID != item.ID)
                {
                    current = new VariationReportsData
                    {
                        ID = item.ID,
                        Name = item.Name,
                        RowNumber = item.RowNumber,
                        Values = new List<decimal>()
                    };
                    variationReportsData.Add(current);

                }
            }

            foreach (var rep in variationReportsData)
            {
                foreach (var timeRange in timeRanges)
                {
                    var value = variationReports.FirstOrDefault(itm => itm.ID == rep.ID && itm.FromDate == timeRange.FromDate && itm.ToDate == timeRange.ToDate);
                    if (value != null)
                        rep.Values.Add(value.TotalDuration);
                    else
                        rep.Values.Add(0);
                }
            }

            foreach (var item in variationReportsData)
            {
                decimal average = 0;
                double CurrentValue = double.Parse(item.Values.First().ToString());
                double PrevValue = double.Parse(item.Values[1].ToString());
                foreach (var totalDurations in item.Values)
                    average += totalDurations;
                average = average / periodCount;
                item.PeriodTypeValueAverage = average;
                item.PeriodTypeValuePercentage = Convert.ToDecimal((CurrentValue - Convert.ToDouble(average)) / (average == 0 ? double.MaxValue : Convert.ToDouble(average))) * 100;
                item.PreviousPeriodTypeValuePercentage = Convert.ToDecimal((CurrentValue - PrevValue) / (PrevValue == 0 ? double.MaxValue : PrevValue)) * 100;

            }

            totals = new List<decimal>();
            int i = 0;

            foreach (var timeRange in timeRanges)
            {
                if (datetotalValues.Contains(timeRange.FromDate))
                {
                    totals.Add(totalValues[i]);
                    i++;
                }
                else
                    totals.Add(0);

            }

            ////Calcule of Total AVG, Total %, Total Previouss %: 
             foreach (var item in variationReportsData)
            {
                decimal average = 0;
                double CurrentDayValue = double.Parse(item.Values.First().ToString());
                double PrevDayValue = double.Parse(item.Values[1].ToString());
                foreach (var totalDurations in item.Values)
                    average += totalDurations;
                average = average / periodCount;
                item.PeriodTypeValueAverage = average;
                item.PeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - Convert.ToDouble(average)) / (average == 0 ? double.MaxValue : Convert.ToDouble(average))) * 100;
                item.PreviousPeriodTypeValuePercentage = Convert.ToDecimal((CurrentDayValue - PrevDayValue) / (PrevDayValue == 0 ? double.MaxValue : PrevDayValue)) * 100;

            }
             double currentValue = double.Parse(totals.FirstOrDefault().ToString());
             double prevValue = double.Parse(totals[1].ToString());
            totalPercentage = Convert.ToDecimal((currentValue - Convert.ToDouble(totalAverage)) / (totalAverage == 0 ? double.MaxValue : Convert.ToDouble(totalAverage))) * 100;
            totalPreviousPercentage = Convert.ToDecimal((currentValue - prevValue) / (prevValue == 0 ? double.MaxValue : prevValue)) * 100;

            return new VariationReportResult() { VariationReportsData = variationReportsData.OrderBy(itm => itm.RowNumber), TimeRange = timeRanges };

        }
        public List<RateLossFormatted> GetRateLoss(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? zoneId, int? customerAMUId, int? supplierAMUId)
        {

            return FormatRateLosses(_datamanager.GetRateLoss(fromDate, toDate, customerId, supplierId, zoneId, supplierAMUId, customerAMUId));
        }

        public List<CarrierSummaryFormatted> GetCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {

            return FormatCarrierSummaries(_datamanager.GetCarrierSummary(fromDate, toDate, customerId, supplierId, supplierAMUId, customerAMUId));
        }

        public List<DetailedCarrierSummaryFormatted> GetDetailedCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int? customerAMUId, int? supplierAMUId)
        {

            return FormatDetailedCarrierSummaries(_datamanager.GetCarrierDetailedSummary(fromDate, toDate, customerId, supplierId, supplierAMUId, customerAMUId));
        }

        public List<CustomerSummaryFormatted> GetCustomerSummary(DateTime fromDate, DateTime toDate, string customerId, int? customerAMUId, int? supplierAMUId)
        {

            List<CustomerSummary> customerSummaries = _datamanager.GetCustomerSummary(fromDate, toDate, customerId, customerAMUId, supplierAMUId);

            List<CustomerServices> customerServices = _datamanager.GetCustomerServices(fromDate, toDate);
            List<CustomerSummaryFormatted> customerSummariesFormatted = new List<CustomerSummaryFormatted>();

            Dictionary<string, double> totalServicesPerCustomer = new Dictionary<string, double>();
            foreach (var obj in customerServices)
            {
                if (obj.AccountId != null && obj.Services != null)
                    totalServicesPerCustomer.Add(obj.AccountId, obj.Services);

            }
            foreach (var cs in customerSummaries)
            {
                CustomerSummaryFormatted entitie = new CustomerSummaryFormatted()
                {
                    Carrier = cs.Carrier,
                    SaleDuration = cs.SaleDuration,
                    SaleDurationFormatted = FormatNumber(cs.SaleDuration),
                    SaleNet = cs.SaleNet,
                    SaleNetFormatted = FormatNumber(cs.SaleNet, 5),
                    CostDuration = cs.CostDuration,
                    CostDurationFormatted = FormatNumber(cs.CostDuration),
                    CostNet = cs.CostNet,
                    CostNetFormatted = FormatNumber(cs.CostNet),
                    ProfitFormatted = FormatNumber(cs.SaleNet - cs.CostNet),
                    ProfitPercentageFormatted = (cs.SaleNet > 0) ? String.Format("{0:#,##0.00%}", ((cs.SaleNet - cs.CostNet) / cs.SaleNet)) : String.Format("{0:#,##0.00%}", 0),

                };
                if (cs.Carrier != null)
                {
                    entitie.Customer = _bemanager.GetCarrirAccountName(cs.Carrier);
                    entitie.Services = (totalServicesPerCustomer.ContainsKey(cs.Carrier)) ? totalServicesPerCustomer[cs.Carrier] : 0;
                    entitie.ServicesFormatted = FormatNumber((totalServicesPerCustomer.ContainsKey(cs.Carrier)) ? totalServicesPerCustomer[cs.Carrier] : 0);
                }
                else
                {
                    entitie.Customer = null;
                    entitie.Services = 0;
                    entitie.ServicesFormatted = FormatNumber(0.00);

                }
                customerSummariesFormatted.Add(entitie);
            }

            return customerSummariesFormatted.OrderBy(cs => cs.Customer).ToList();
        }

        public List<ProfitSummary> GetCustomerProfitSummary(List<CustomerSummaryFormatted> summarieslist)
        {
            List<ProfitSummary> profitsummaries = new List<ProfitSummary>();

            foreach (var cs in summarieslist)
            {
                ProfitSummary p = new ProfitSummary()
                {
                    Profit = (cs.SaleNet != null ? (double)cs.SaleNet : 0 - (cs.CostNet != null ? (double)cs.CostNet : 0)),
                    FormattedProfit = string.Format("{0:#,##0.00}", (cs.SaleNet != null ? (double)cs.SaleNet : 0) - (cs.CostNet != null ? (double)cs.CostNet : 0)),
                    Customer = cs.Customer
                };
                profitsummaries.Add(p);
            }
            return profitsummaries.OrderByDescending(r => r.Profit).Take(10).ToList();
        }
        public List<SaleAmountSummary> GetCustomerSaleAmountSummary(List<CustomerSummaryFormatted> summarieslist)
        {
            List<SaleAmountSummary> saleAmountSummary = new List<SaleAmountSummary>();

            foreach (var cs in summarieslist)
            {
                SaleAmountSummary s = new SaleAmountSummary()
                {
                    SaleAmount = cs.SaleNet != null ? (double)cs.SaleNet : 0,
                    FormattedSaleAmount = string.Format("{0:#,##0.00}", cs.SaleNet != null ? (double)cs.SaleNet : 0),
                    Customer = cs.Customer
                };
                saleAmountSummary.Add(s);
            }
            return saleAmountSummary.OrderByDescending(s => s.SaleAmount).Take(10).ToList();
        }

        public List<DailyForcastingFormatted> GetDailyForcasting(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatDailyForcastingSummaries(_datamanager.GetDailyForcasting(fromDate, toDate, supplierAMUId, customerAMUId));
        }
        public List<ExchangeCarrierFormatted> GetExchangeCarriers(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId, bool IsExchange)
        {
            List<ExchangeCarriers> baseList = _datamanager.GetExchangeCarriers(fromDate, toDate, supplierAMUId, customerAMUId);
            List<ExchangeCarrierProfit> ecpList = new List<ExchangeCarrierProfit>();
            CarrierManager ca = new CarrierManager();
            List<CarrierAccount> list = ca.GetAllCarriers();
            List<ExchangeCarrierFormatted> result = new List<ExchangeCarrierFormatted>();
            foreach (var row in baseList)
            {

                ExchangeCarrierProfit ecp = new ExchangeCarrierProfit()
                {
                    CarrierAccount = ca.GetCarrierAccount(row.CustomerID),// list.Where(s => s.CarrierAccountId == row.CustomerID ).FirstOrDefault(),
                    CustomerProfit = row.CustomerProfit != null ? (double)row.CustomerProfit : 0,
                    SupplierProfit = row.SupplierProfit != null ? (double)row.SupplierProfit : 0

                };
                ecpList.Add(ecp);
            }
            var test = ecpList;
            if (IsExchange == true)
            {
                result = ecpList.Where(c => (AccountType)c.CarrierAccount.AccountType == AccountType.Exchange)
                .Select(c =>
                new ExchangeCarrierFormatted
                {
                    Customer = c.CarrierAccount.NameSuffix,
                    CustomerProfit = c.CustomerProfit,
                    SupplierProfit = c.SupplierProfit,
                    FormattedCustomerProfit = FormatNumber(c.CustomerProfit),
                    FormattedSupplierProfit = FormatNumber(c.SupplierProfit),
                    Total = FormatNumber(c.CustomerProfit + c.SupplierProfit)

                }).OrderByDescending(r => r.CustomerProfit + r.SupplierProfit).ToList();

            }
            else
            {

                result = ecpList.GroupBy(c => c.CarrierAccount).Select(obj => new ExchangeCarrierFormatted
                {
                    Customer = obj.Key.ProfileName,
                    CustomerProfit = obj.Sum(d => d.CustomerProfit),
                    SupplierProfit = obj.Sum(d => d.SupplierProfit),
                    FormattedCustomerProfit = FormatNumber(obj.Sum(d => d.CustomerProfit)),
                    FormattedSupplierProfit = FormatNumber(obj.Sum(d => d.SupplierProfit)),
                    Total = FormatNumber(obj.Sum(d => d.CustomerProfit) + obj.Sum(d => d.SupplierProfit))

                }).OrderByDescending(r => r.CustomerProfit + r.SupplierProfit).ToList();


            }

            return result;


        }
        #region Private Methods
        private ZoneProfitFormatted FormatZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormatted
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID = (zoneProfit.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneProfit.SupplierID) : null,
                CustomerID = (zoneProfit.CustomerID != null) ? _bemanager.GetCarrirAccountName(zoneProfit.CustomerID) : null,
                Calls = zoneProfit.Calls,

                DurationNet = zoneProfit.DurationNet,
                DurationNetFormated = FormatNumber(zoneProfit.DurationNet),

                SaleDuration = zoneProfit.SaleDuration,
                SaleDurationFormated = (zoneProfit.SaleDuration.HasValue) ? String.Format("{0:#0.00}", zoneProfit.SaleDuration) : "0.00",

                SaleNet = zoneProfit.SaleNet,
                SaleNetFormated = (zoneProfit.SaleNet.HasValue) ? String.Format("{0:#0.00}", zoneProfit.SaleNet) : "0.00",

                CostDuration = zoneProfit.CostDuration,
                CostDurationFormated = String.Format("{0:#0.0000}", zoneProfit.CostDuration),

                CostNet = zoneProfit.CostNet,
                CostNetFormated = (zoneProfit.CostNet.HasValue) ? String.Format("{0:#0.0000}", zoneProfit.CostNet) : "0.00",

                Profit = String.Format("{0:#0.00}", (zoneProfit.SaleNet - zoneProfit.CostNet)),

                ProfitPercentage = (zoneProfit.SaleNet.HasValue) ? String.Format("{0:#,##0.00%}", (1 - zoneProfit.CostNet / zoneProfit.SaleNet)) : "-100%",


            };
        }

        private ZoneSummaryFormatted FormatZoneSummary(ZoneSummary zoneSummary)
        {
            return new ZoneSummaryFormatted
            {
                Zone = zoneSummary.Zone,
                SupplierID = (zoneSummary.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneSummary.SupplierID) : null,

                Calls = zoneSummary.Calls,

                Rate = zoneSummary.Rate,
                RateFormatted = FormatNumber(zoneSummary.Rate, 5),

                DurationNet = zoneSummary.DurationNet,
                DurationNetFormatted = FormatNumber(zoneSummary.DurationNet),

                RateType = zoneSummary.RateType,
                RateTypeFormatted = ((RateType)zoneSummary.RateType).ToString(),

                DurationInSeconds = zoneSummary.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummary.DurationInSeconds),

                Net = zoneSummary.Net,
                NetFormatted = FormatNumber(zoneSummary.Net, 5),

                CommissionValue = zoneSummary.CommissionValue,
                CommissionValueFormatted = FormatNumber(zoneSummary.CommissionValue),

                ExtraChargeValue = zoneSummary.ExtraChargeValue
            };
        }

        private ZoneSummaryDetailedFormatted FormatZoneSummaryDetailed(ZoneSummaryDetailed zoneSummaryDetailed)
        {
            return new ZoneSummaryDetailedFormatted
            {
                Zone = zoneSummaryDetailed.Zone,
                ZoneId = zoneSummaryDetailed.ZoneId,

                SupplierID = (zoneSummaryDetailed.SupplierID != null) ? _bemanager.GetCarrirAccountName(zoneSummaryDetailed.SupplierID) : null,

                Calls = zoneSummaryDetailed.Calls,

                Rate = zoneSummaryDetailed.Rate,
                RateFormatted = FormatNumber(zoneSummaryDetailed.Rate, 5),

                DurationNet = zoneSummaryDetailed.DurationNet,
                DurationNetFormatted = FormatNumber(zoneSummaryDetailed.DurationNet),

                OffPeakDurationInSeconds = zoneSummaryDetailed.OffPeakDurationInSeconds,
                OffPeakDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.OffPeakDurationInSeconds, 2),

                OffPeakRate = zoneSummaryDetailed.OffPeakRate,
                OffPeakRateFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate, 5),

                OffPeakNet = zoneSummaryDetailed.OffPeakNet,
                OffPeakNetFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate, 2),

                WeekEndDurationInSeconds = zoneSummaryDetailed.WeekEndDurationInSeconds,
                WeekEndDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.WeekEndDurationInSeconds, 2),

                WeekEndRate = zoneSummaryDetailed.WeekEndRate,
                WeekEndRateFormatted = FormatNumber(zoneSummaryDetailed.WeekEndRate, 2),

                WeekEndNet = zoneSummaryDetailed.WeekEndNet,
                WeekEndNetFormatted = FormatNumber(zoneSummaryDetailed.WeekEndNet, 2),

                DurationInSeconds = zoneSummaryDetailed.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.DurationInSeconds),

                Net = zoneSummaryDetailed.Net,
                NetFormatted = FormatNumber(zoneSummaryDetailed.Net, 5),

                TotalDurationFormatted = FormatNumber((zoneSummaryDetailed.DurationInSeconds + zoneSummaryDetailed.OffPeakDurationInSeconds + zoneSummaryDetailed.WeekEndDurationInSeconds), 2),

                TotalAmountFormatted = FormatNumber(zoneSummaryDetailed.Net + zoneSummaryDetailed.OffPeakNet + zoneSummaryDetailed.WeekEndNet, 2),

                CommissionValue = zoneSummaryDetailed.CommissionValue,
                CommissionValueFormatted = FormatNumber(zoneSummaryDetailed.CommissionValue, 2),

                ExtraChargeValue = zoneSummaryDetailed.ExtraChargeValue,
                ExtraChargeValueFormatted = FormatNumber(zoneSummaryDetailed.ExtraChargeValue),
            };
        }

        private DailySummaryFormatted FormatDailySummary(DailySummary dailySummary)
        {
            return new DailySummaryFormatted
            {
                Day = dailySummary.Day,
                Calls = dailySummary.Calls,

                DurationNet = dailySummary.DurationNet,
                DurationNetFormatted = FormatNumber(dailySummary.DurationNet),

                SaleDuration = dailySummary.SaleDuration,
                SaleDurationFormatted = FormatNumber(dailySummary.SaleDuration, 2),

                SaleNet = dailySummary.SaleNet,
                SaleNetFormatted = FormatNumber(dailySummary.SaleNet, 5),

                CostNet = dailySummary.CostNet,
                CostNetFormatted = FormatNumber(dailySummary.CostNet, 2),

                ProfitFormatted = FormatNumber(dailySummary.SaleNet - dailySummary.CostNet, 2),


                ProfitPercentageFormatted = (dailySummary.SaleNet.HasValue) ? String.Format("{0:#,##0.00%}", (1 - dailySummary.CostNet / dailySummary.SaleNet)) : "-100%",
            };
        }

        private CarrierLostFormatted FormatCarrierLost(CarrierLost carrierLost)
        {

            return new CarrierLostFormatted
            {
                CustomerID = carrierLost.CustomerID,
                CustomerName = _bemanager.GetCarrirAccountName(carrierLost.CustomerID),
                SupplierID = carrierLost.SupplierID,
                SupplierName = _bemanager.GetCarrirAccountName(carrierLost.SupplierID),
                SaleZoneID = carrierLost.SaleZoneID,
                SaleZoneName = _bemanager.GetZoneName(carrierLost.SaleZoneID),
                CostZoneID = carrierLost.CostZoneID,
                CostZoneName = _bemanager.GetZoneName(carrierLost.CostZoneID),
                Duration = carrierLost.Duration,
                DurationFormatted = FormatNumber(carrierLost.Duration),
                CostNet = carrierLost.CostNet,
                CostNetFormatted = (carrierLost.CostNet == null) ? "0.00" : FormatNumber(carrierLost.CostNet, 5),
                SaleNet = carrierLost.SaleNet,
                SaleNetFormatted = (carrierLost.SaleNet == null) ? "0.00" : FormatNumber(carrierLost.SaleNet, 5),
                Margin = FormatNumber(carrierLost.SaleNet - carrierLost.CostNet),
                Percentage = (carrierLost.SaleNet != null) ? String.Format("{0:#,##0.00%}", (1 - carrierLost.CostNet / carrierLost.SaleNet)) : "-100%"

            };
        }
        private List<RateLossFormatted> FormatRateLosses(List<RateLoss> rateLosses)
        {
            List<RateLossFormatted> models = new List<RateLossFormatted>();
            if (rateLosses != null)
                foreach (var z in rateLosses)
                {
                    models.Add(FormatRateLoss(z));
                }
            return models;
        }
        private RateLossFormatted FormatRateLoss(RateLoss rateLoss)
        {
            return new RateLossFormatted
           {
               CostZone = rateLoss.CostZone,
               SaleZone = rateLoss.SaleZone,
               CustomerID = rateLoss.CustomerID,
               Customer = (rateLoss.CustomerID != null) ? _bemanager.GetCarrirAccountName(rateLoss.CustomerID) : null,
               SupplierID = rateLoss.SupplierID,
               Supplier = (rateLoss.SupplierID != null) ? _bemanager.GetCarrirAccountName(rateLoss.SupplierID) : null,
               SaleRate = rateLoss.SaleRate,
               SaleRateFormatted = FormatNumber(rateLoss.SaleRate, 5),
               CostRate = rateLoss.CostRate,
               CostRateFormatted = FormatNumber(rateLoss.CostRate, 5),
               CostDuration = rateLoss.CostDuration,
               CostDurationFormatted = FormatNumber(rateLoss.CostDuration),
               SaleDuration = rateLoss.SaleDuration,
               SaleDurationFormatted = FormatNumber(rateLoss.SaleDuration),
               CostNet = rateLoss.CostNet,
               CostNetFormatted = FormatNumber(rateLoss.CostNet, 5),
               SaleNet = rateLoss.SaleNet,
               SaleNetFormatted = FormatNumber(rateLoss.SaleNet, 5),
               SaleZoneID = rateLoss.SaleZoneID,
               LossFormatted = FormatNumber(rateLoss.CostNet - rateLoss.SaleNet)

           };
        }
        private string FormatNumber(Decimal? number, int precision)
        {
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }
        private string FormatNumber(Double? number, int precision)
        {
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        private string FormatNumber(Decimal? number)
        {
            return String.Format("{0:#0.00}", number);
        }

        private string FormatNumber(Double? number)
        {
            return String.Format("{0:#0.00}", number);
        }
        private List<ZoneProfitFormatted> FormatZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormatted> models = new List<ZoneProfitFormatted>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(FormatZoneProfit(z));
                }
            return models;
        }

        private List<ZoneSummaryFormatted> FormatZoneSummaries(List<ZoneSummary> zoneSummaries)
        {
            List<ZoneSummaryFormatted> models = new List<ZoneSummaryFormatted>();
            if (zoneSummaries != null)
                foreach (var z in zoneSummaries)
                {
                    models.Add(FormatZoneSummary(z));
                }
            return models;
        }
        private List<ZoneSummaryDetailedFormatted> FormatZoneSummariesDetailed(List<ZoneSummaryDetailed> zoneSummariesDetailed)
        {
            List<ZoneSummaryDetailedFormatted> models = new List<ZoneSummaryDetailedFormatted>();
            if (zoneSummariesDetailed != null)
                foreach (var z in zoneSummariesDetailed)
                {
                    models.Add(FormatZoneSummaryDetailed(z));
                }
            return models;
        }

        private List<DailySummaryFormatted> FormatDailySummaries(List<DailySummary> dailySummary)
        {
            List<DailySummaryFormatted> models = new List<DailySummaryFormatted>();
            if (dailySummary != null)
                foreach (var z in dailySummary)
                {
                    models.Add(FormatDailySummary(z));
                }
            return models;
        }
        private List<CarrierLostFormatted> FormatCarrierLost(List<CarrierLost> carriersLost)
        {

            List<CarrierLostFormatted> models = new List<CarrierLostFormatted>();
            if (carriersLost != null)
                foreach (var z in carriersLost)
                {
                    models.Add(FormatCarrierLost(z));
                }
            return models;

        }

        private List<CarrierSummaryDailyFormatted> FormatCarrieresSummaryDaily(List<CarrierSummaryDaily> carrierSummaryDaily, bool isGroupeByday)
        {

            List<CarrierSummaryDailyFormatted> models = new List<CarrierSummaryDailyFormatted>();
            if (carrierSummaryDaily != null)
                foreach (var z in carrierSummaryDaily)
                {
                    models.Add(FormatCarrierSummaryDaily(z, isGroupeByday));
                }
            return models;

        }
        private List<DailyForcastingFormatted> FormatDailyForcastingSummaries(List<DailyForcasting> carrierDaily)
        {

            List<DailyForcastingFormatted> models = new List<DailyForcastingFormatted>();
            if (carrierDaily != null)
                foreach (var d in carrierDaily)
                {
                    models.Add(FormatDailyForcasting(d));
                }
            return models;

        }

        private CarrierSummaryDailyFormatted FormatCarrierSummaryDaily(CarrierSummaryDaily carrierSummaryDaily, bool isGroupeByday)
        {

            CarrierSummaryDailyFormatted obj = new CarrierSummaryDailyFormatted
            {

                Day = carrierSummaryDaily.Day,
                CarrierID = carrierSummaryDaily.CarrierID,
                Carrier = _bemanager.GetCarrirAccountName(carrierSummaryDaily.CarrierID),
                Attempts = carrierSummaryDaily.Attempts,
                DurationNet = carrierSummaryDaily.DurationNet,
                DurationNetFormatted = FormatNumber(carrierSummaryDaily.DurationNet),
                Duration = carrierSummaryDaily.Duration,
                DurationFormatted = FormatNumber(carrierSummaryDaily.Duration),
                Net = carrierSummaryDaily.Net,
                NetFormatted = FormatNumber(carrierSummaryDaily.Net, 5)

            };
            if (isGroupeByday)
                obj.Date = obj.Day.ToString();

            return obj;
        }

        private List<CarrierSummaryFormatted> FormatCarrierSummaries(List<CarrierSummary> carrierSummaries)
        {
            List<CarrierSummaryFormatted> models = new List<CarrierSummaryFormatted>();
            if (carrierSummaries != null)
                foreach (var c in carrierSummaries)
                {
                    models.Add(FormatCarrierSummary(c));
                }
            return models;
        }
        private List<DetailedCarrierSummaryFormatted> FormatDetailedCarrierSummaries(List<DetailedCarrierSummary> detailedcarrierSummaries)
        {
            List<DetailedCarrierSummaryFormatted> models = new List<DetailedCarrierSummaryFormatted>();
            if (detailedcarrierSummaries != null)
                foreach (var c in detailedcarrierSummaries)
                {
                    models.Add(FormatDetailedCarrierSummary(c));
                }
            return models.OrderBy(cs => cs.Customer).ToList();
        }

        private DailyForcastingFormatted FormatDailyForcasting(DailyForcasting daily)
        {
            return new DailyForcastingFormatted
            {
                Day = daily.Day,
                SaleNet = daily.SaleNet,
                SaleNetFormatted = FormatNumber(daily.SaleNet, 5),
                CostNet = daily.CostNet,
                CostNetFormatted = FormatNumber(daily.CostNet, 5),
                ProfitFormatted = FormatNumber(daily.SaleNet - daily.CostNet, 2),
                ProfitPercentageFormatted = (daily.SaleNet > 0) ? String.Format("{0:#,##0.00%}", ((daily.SaleNet - daily.CostNet) / daily.SaleNet)) : "0.00%"

            };
        }
        private CarrierSummaryFormatted FormatCarrierSummary(CarrierSummary carrierSummary)
        {
            return new CarrierSummaryFormatted
            {

                SupplierID = carrierSummary.SupplierID,
                Supplier = (carrierSummary.SupplierID != null) ? _bemanager.GetCarrirAccountName(carrierSummary.SupplierID) : null,
                CustomerID = carrierSummary.CustomerID,
                Customer = (carrierSummary.CustomerID != null) ? _bemanager.GetCarrirAccountName(carrierSummary.CustomerID) : null,
                SaleDuration = carrierSummary.SaleDuration,
                SaleDurationFormatted = FormatNumber(carrierSummary.SaleDuration),
                CostDuration = carrierSummary.CostDuration,
                CostDurationFormatted = FormatNumber(carrierSummary.CostDuration),
                CostNet = carrierSummary.CostNet,
                CostNetFormatted = (carrierSummary.CostNet == 0.00) ? "0.00" : FormatNumber(carrierSummary.CostNet, 5),
                SaleNet = carrierSummary.SaleNet,
                SaleNetFormatted = (carrierSummary.SaleNet == 0.00) ? "0.00" : FormatNumber(carrierSummary.SaleNet, 5),
                CostCommissionValue = carrierSummary.CostCommissionValue,
                CostCommissionValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.CostCommissionValue.Value))),
                SaleCommissionValue = carrierSummary.SaleCommissionValue,
                SaleCommissionValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.SaleCommissionValue.Value))),
                CostExtraChargeValue = carrierSummary.CostExtraChargeValue,
                CostExtraChargeValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.CostExtraChargeValue.Value))),
                SaleExtraChargeValue = carrierSummary.SaleExtraChargeValue,
                SaleExtraChargeValueFormatted = FormatNumber(Convert.ToDouble(Math.Abs(carrierSummary.SaleExtraChargeValue.Value))),
                Profit = FormatNumber(carrierSummary.SaleNet - carrierSummary.CostNet),
                AvgMin = (carrierSummary.SaleDuration.Value != 0 && carrierSummary.SaleDuration.Value != 0) ? FormatNumber((decimal)carrierSummary.SaleNet / carrierSummary.SaleDuration - (decimal)carrierSummary.CostNet / carrierSummary.SaleDuration) : "0.00"


            };
        }

        private DetailedCarrierSummaryFormatted FormatDetailedCarrierSummary(DetailedCarrierSummary detailedCarrier)
        {
            return new DetailedCarrierSummaryFormatted
            {

                CustomerID = detailedCarrier.CustomerID,
                Customer = (detailedCarrier.CustomerID != null) ? _bemanager.GetCarrirAccountName(detailedCarrier.CustomerID) : null,
                SaleZoneID = detailedCarrier.SaleZoneID,
                SaleZoneName = detailedCarrier.SaleZoneName,
                SaleDuration = detailedCarrier.SaleDuration,
                SaleDurationFormatted = FormatNumber(detailedCarrier.SaleDuration),
                SaleRate = detailedCarrier.SaleRate,
                SaleRateFormatted = FormatNumber(detailedCarrier.SaleRate, 5),
                SaleRateChange = detailedCarrier.SaleRateChange,
                SaleRateChangeFormatted = ((TOne.Analytics.Entities.Change)detailedCarrier.SaleRateChange).ToString(),
                SaleRateEffectiveDate = detailedCarrier.SaleRateEffectiveDate,
                SaleAmount = detailedCarrier.SaleAmount,
                SaleAmountFormatted = FormatNumber(detailedCarrier.SaleAmount, 5),
                SupplierID = detailedCarrier.SupplierID,
                Supplier = (detailedCarrier.SupplierID != null) ? _bemanager.GetCarrirAccountName(detailedCarrier.SupplierID) : null,
                CostZoneID = detailedCarrier.CostZoneID,
                CostZoneName = detailedCarrier.CostZoneName,
                CostDuration = detailedCarrier.CostDuration,
                CostDurationFormatted = FormatNumber(detailedCarrier.CostDuration),
                CostRate = detailedCarrier.CostRate,
                CostRateFormatted = FormatNumber(detailedCarrier.CostRate, 5),
                CostRateChange = detailedCarrier.CostRateChange,
                CostRateChangeFormatted = ((TOne.Analytics.Entities.Change)detailedCarrier.CostRateChange).ToString(),
                CostRateEffectiveDate = detailedCarrier.CostRateEffectiveDate,
                CostAmount = detailedCarrier.CostAmount,
                CostAmountFormatted = FormatNumber(detailedCarrier.CostAmount, 5),
                Profit = detailedCarrier.Profit,
                ProfitFormatted = FormatNumber(detailedCarrier.Profit, 5),
                ProfitPercentage = (detailedCarrier.SaleAmount > 0) ? String.Format("{0:#,##0.00%}", (1 - detailedCarrier.CostAmount / detailedCarrier.SaleAmount)) : "-100%"
            };
        }

        #endregion
    }
}
