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
        public List<ZoneProfitFormatted> GetZoneProfit(DateTime fromDate, DateTime toDate, string groupByCustomer)
        {

            return FormatZoneProfits(_datamanager.GetZoneProfit(fromDate, toDate, groupByCustomer));
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
                DurationNetFormated = String.Format("{0:#0.00}", zoneProfit.DurationNet),

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

    }
}
