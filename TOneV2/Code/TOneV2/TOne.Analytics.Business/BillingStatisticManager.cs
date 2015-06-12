﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Data;
using TOne.Analytics.Entities;
using TOne.BusinessEntity.Business;

namespace TOne.Analytics.Business
{
    public class BillingStatisticManager
    {
        private readonly IBillingStatisticDataManager _datamanager;
        private readonly BusinessEntityInfoManager _bemanager;

        public BillingStatisticManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
            _bemanager = new BusinessEntityInfoManager();
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate,  bool groupByCustomer)
        {

            return GetZoneProfit(fromDate, toDate, null, null, groupByCustomer, null, null);
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate,string customerId ,string supplierId , bool groupByCustomer , int? supplierAMUId ,int? customerAMUId)
        {

            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, customerId , supplierId ,  groupByCustomer ,  supplierAMUId , customerAMUId));
        }

        public List<DailySummaryFormatted> GetDailySummary(DateTime fromDate, DateTime toDate, int? customerAMUId, int? supplierAMUId)
        {
            return FormatDailySummaries(_datamanager.GetDailySummary(fromDate, toDate, customerAMUId, supplierAMUId));
        }

        public List<ZoneSummaryFormatted> GetZoneSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return FormatZoneSummaries(_datamanager.GetZoneSummary( fromDate,  toDate,  customerId, supplierId,  isCost, currencyId,  supplierGroup,  customerGroup,  customerAMUId,  supplierAMUId,  groupBySupplier));
        }
        public List<ZoneSummaryDetailedFormatted> GetZoneSummaryDetailed(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, string currencyId, string supplierGroup, string customerGroup, int? customerAMUId, int? supplierAMUId, bool groupBySupplier)
        {
            return FormatZoneSummariesDetailed(_datamanager.GetZoneSummaryDetailed(fromDate, toDate, customerId, supplierId, isCost, currencyId, supplierGroup, customerGroup, customerAMUId, supplierAMUId, groupBySupplier));
        }

        public List<CarrierLostFormatted> GetCarrierLost(DateTime fromDate, DateTime toDate, string customerId, string supplierId, int margin, int? supplierAMUId, int? customerAMUId)
        {
            return FormatCarrierLost(_datamanager.GetCarrierLost(fromDate,toDate,customerId,supplierId,margin,supplierAMUId,customerAMUId));
        }       
        public List<MonthTraffic>GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {

            return _datamanager.GetMonthTraffic(fromDate, toDate, carrierAccountID, isSale);
        }

        public List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount)
        {
            return _datamanager.GetCarrierProfile(fromDate, toDate, carrierAccountID, TopDestinations, isSale, IsAmount);
        }

        public List<BillingStatistic> GetBillingStatistics(DateTime fromDate, DateTime toDate)
        {

            return _datamanager.GetBillingStatistics(fromDate,toDate);
        }
        public List<CarrierSummaryDailyFormatted> GetDailyCarrierSummary(DateTime fromDate, DateTime toDate, string customerId, string supplierId, bool isCost, bool isGroupedByDay, int? customerAMUId, int? supplierAMUId)
        {
            return FormatCarrieresSummaryDaily(_datamanager.GetDailyCarrierSummary(fromDate, toDate, customerId, supplierId, isCost, isGroupedByDay, supplierAMUId, customerAMUId), isGroupedByDay);
        }

        //public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue)
        //{
        //    return _datamanager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
        //}

        public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue, int variationReportOptionValue)
        {
            return _datamanager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue,variationReportOptionValue);
        }

        

        #region Private Methods
        private ZoneProfitFormatted FormatZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormatted
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID =(zoneProfit.SupplierID!=null)? _bemanager.GetCarrirAccountName(zoneProfit.SupplierID):null,
                CustomerID = (zoneProfit.CustomerID!= null) ? _bemanager.GetCarrirAccountName(zoneProfit.CustomerID) : null,
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
                SupplierID = (zoneSummary.SupplierID!=null)?_bemanager.GetCarrirAccountName(zoneSummary.SupplierID):null,

                Calls = zoneSummary.Calls,

                Rate = zoneSummary.Rate,
                RateFormatted = FormatNumber(zoneSummary.Rate,5),

                DurationNet = zoneSummary.DurationNet,
                DurationNetFormatted = FormatNumber(zoneSummary.DurationNet),

                RateType = zoneSummary.RateType,
                RateTypeFormatted = ((RateType)zoneSummary.RateType).ToString(),

                DurationInSeconds = zoneSummary.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummary.DurationInSeconds),

                Net =zoneSummary.Net,
                NetFormatted = FormatNumber(zoneSummary.Net,5),

                CommissionValue =zoneSummary.CommissionValue,
                CommissionValueFormatted= FormatNumber(zoneSummary.CommissionValue),

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
                OffPeakDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.OffPeakDurationInSeconds,2),

                OffPeakRate = zoneSummaryDetailed.OffPeakRate,
                OffPeakRateFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate,5),

                OffPeakNet = zoneSummaryDetailed.OffPeakNet,
                OffPeakNetFormatted = FormatNumber(zoneSummaryDetailed.OffPeakRate,2),

                WeekEndDurationInSeconds = zoneSummaryDetailed.WeekEndDurationInSeconds,
                WeekEndDurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.WeekEndDurationInSeconds,2),

                WeekEndRate = zoneSummaryDetailed.WeekEndRate,
                WeekEndRateFormatted = FormatNumber(zoneSummaryDetailed.WeekEndRate,2),

                WeekEndNet = zoneSummaryDetailed.WeekEndNet,
                WeekEndNetFormatted = FormatNumber(zoneSummaryDetailed.WeekEndNet,2),

                DurationInSeconds = zoneSummaryDetailed.DurationInSeconds,
                DurationInSecondsFormatted = FormatNumber(zoneSummaryDetailed.DurationInSeconds),

                Net = zoneSummaryDetailed.Net,
                NetFormatted = FormatNumber(zoneSummaryDetailed.Net, 5),

                TotalDurationFormatted = FormatNumber((zoneSummaryDetailed.DurationInSeconds + zoneSummaryDetailed.OffPeakDurationInSeconds + zoneSummaryDetailed.WeekEndDurationInSeconds),2),

                TotalAmountFormatted = FormatNumber(zoneSummaryDetailed.Net + zoneSummaryDetailed.OffPeakNet + zoneSummaryDetailed.WeekEndNet,2),

                CommissionValue = zoneSummaryDetailed.CommissionValue,
                CommissionValueFormatted = FormatNumber(zoneSummaryDetailed.CommissionValue,2),

                ExtraChargeValue = zoneSummaryDetailed.ExtraChargeValue ,
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

                ProfitFormatted = FormatNumber(dailySummary.SaleNet - dailySummary.CostNet , 2),

                //=IIf(IsNothing(Fields!SaleNet.Value) ,"-100%",FormatPercent(1- Fields!CostNet.Value/Fields!SaleNet.Value,2))

                ProfitPercentageFormatted = (dailySummary.SaleNet.HasValue) ? String.Format("{0:#,##0.00%}", (1 - dailySummary.CostNet / dailySummary.SaleNet)) : "-100%",
                //ProfitPercentageFormatted = dailySummary.SaleNet == null ? "-100%" : FormatNumber(1 - dailySummary.CostNet / dailySummary.SaleNet, 2)
            };
        }

        private CarrierLostFormatted FormatCarrierLost(CarrierLost carrierLost)
        {
            
            return new CarrierLostFormatted
            {
              CustomerID = carrierLost.CustomerID ,
              CustomerName = _bemanager.GetCarrirAccountName(carrierLost.CustomerID),
              SupplierID = carrierLost.SupplierID,
              SupplierName = _bemanager.GetCarrirAccountName(carrierLost.SupplierID),
              SaleZoneID = carrierLost.SaleZoneID ,
              SaleZoneName = _bemanager.GetZoneName(carrierLost.SaleZoneID),
              CostZoneID = carrierLost.CostZoneID ,
              CostZoneName = _bemanager.GetZoneName(carrierLost.CostZoneID),
              Duration = carrierLost.Duration ,
              DurationFormatted = FormatNumber(carrierLost.Duration),
              CostNet = carrierLost.CostNet,
              CostNetFormatted = (carrierLost.CostNet==null)?"0.00":FormatNumber(carrierLost.CostNet,5),
              SaleNet = carrierLost.SaleNet,
              SaleNetFormatted = (carrierLost.SaleNet == null) ? "0.00" : FormatNumber(carrierLost.SaleNet, 5),
              Margin = FormatNumber(carrierLost.SaleNet - carrierLost.CostNet),
              Percentage = (carrierLost.SaleNet !=null) ? String.Format("{0:#,##0.00%}", (1 - carrierLost.CostNet / carrierLost.SaleNet)) : "-100%"
              
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
        private List<CarrierLostFormatted> FormatCarrierLost(List<CarrierLost> carriersLost) {

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

        #endregion 
    }
}
