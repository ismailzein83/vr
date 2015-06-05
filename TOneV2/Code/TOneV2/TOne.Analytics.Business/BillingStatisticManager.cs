using System;
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

        public BillingStatisticManager()
        {
            _datamanager = AnalyticsDataManagerFactory.GetDataManager<IBillingStatisticDataManager>();
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate,  bool groupByCustomer)
        {

            return GetZoneProfit(fromDate, toDate, null, null, groupByCustomer, null, null);
        }
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate,string customerId ,string supplierId , bool groupByCustomer , int? supplierAMUId ,int? customerAMUId)
        {

            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, customerId , supplierId ,  groupByCustomer ,  supplierAMUId , customerAMUId));
        }

        public List<MonthTraffic>GetMonthTraffic(DateTime fromDate, DateTime toDate, string carrierAccountID, bool isSale)
        {

            return _datamanager.GetMonthTraffic(fromDate, toDate, carrierAccountID, isSale);
        }

        public List<CarrierProfile> GetCarrierProfile(DateTime fromDate, DateTime toDate, string carrierAccountID, int TopDestinations, bool isSale, bool IsAmount)
        {
            return _datamanager.GetCarrierProfile(fromDate, toDate, carrierAccountID, TopDestinations, isSale, IsAmount);
        }


        private ZoneProfitFormatted FormatZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormatted
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID = zoneProfit.SupplierID,
                CustomerID = zoneProfit.CustomerID,
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

        private string FormatNumber(Decimal? number, int precision)
        {
            return String.Format("{0:#0." + "".PadLeft(precision, '0') + "}", number);
        }

        private string FormatNumber(Decimal? number)
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

        public List<BillingStatistic> GetBillingStatistics(DateTime fromDate, DateTime toDate)
        {

            return _datamanager.GetBillingStatistics(fromDate,toDate);
        }

        public List<VariationReports> GetVariationReportsData(DateTime selectedDate, int periodCount, string periodTypeValue)
        {
            return _datamanager.GetVariationReportsData(selectedDate, periodCount, periodTypeValue);
        }

    }
}
