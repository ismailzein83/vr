using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Entities;
using TOne.Data.SQL;

namespace TOne.Analytics.Data.SQL
{
    class BillingStatisticDataManager : BaseTOneDataManager, IBillingStatisticDataManager
    {
        public List<ZoneProfit> GetZoneProfit(DateTime fromDate, DateTime toDate, string groupByCustomer)
        {
            return GetItemsSP("Analytics.sp_billing_GetZoneProfits", (reader) => ZoneProfitMapper(reader, groupByCustomer), fromDate, toDate, null, null, groupByCustomer, null, null);
        }

        private ZoneProfit ZoneProfitMapper(IDataReader reader, string groupByCustomer)
        {
            ZoneProfit instance = new ZoneProfit
            {
                CostZone = reader["CostZone"] as string,
                SaleZone = reader["SaleZone"] as string,
                SupplierID = reader["SupplierID"] as string,
                Calls = GetReaderValue<int>(reader, "Calls"),
                SaleDuration = GetReaderValue<decimal>(reader, "SaleDuration"),
                SaleNet = GetReaderValue<double>(reader, "SaleNet"),
                CostNet = GetReaderValue<double>(reader, "CostNet"),
                CostDuration = GetReaderValue<decimal>(reader, "CostDuration"),
                DurationNet = GetReaderValue<decimal>(reader, "DurationNet")
            };
            if (groupByCustomer == "Y")
            {
                instance.CustomerID = reader["CustomerID"] as string;

            }
            return instance;
        }

        private  ZoneProfitFormatted MapZoneProfit(ZoneProfit zoneProfit)
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

        private  List<ZoneProfitFormatted> MapZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormatted> models = new List<ZoneProfitFormatted>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(MapZoneProfit(z));
                }
            return models;
        }

    }
}
