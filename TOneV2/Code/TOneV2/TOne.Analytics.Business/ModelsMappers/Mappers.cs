using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Analytics.Business.Models;
using TOne.Analytics.Entities;

namespace TOne.Analytics.Business.ModelsMappers
{
    public static class Mappers
    {

        public static ZoneProfitFormated MapZoneProfit(ZoneProfit zoneProfit)
        {
            return new ZoneProfitFormated
            {
                CostZone = zoneProfit.CostZone,
                SaleZone = zoneProfit.SaleZone,
                SupplierID = zoneProfit.SupplierID,
                CustomerID = zoneProfit.CustomerID,
                Calls = zoneProfit.Calls,

                DurationNet =  zoneProfit.DurationNet,
                DurationNetFormated = String.Format("{0:#0.00}", zoneProfit.DurationNet),

                SaleDuration = zoneProfit.SaleDuration,
                SaleDurationFormated = (zoneProfit.SaleDuration.HasValue)?String.Format("{0:#0.00}", zoneProfit.SaleDuration):"0.00",

                SaleNet = zoneProfit.SaleNet,
                SaleNetFormated =(zoneProfit.SaleNet.HasValue) ? String.Format("{0:#0.00}", zoneProfit.SaleNet) : "0.00",

                CostDuration = zoneProfit.CostDuration ,
                CostDurationFormated = String.Format("{0:#0.0000}", zoneProfit.CostDuration),

                CostNet = zoneProfit.CostNet ,
                CostNetFormated = (zoneProfit.CostNet.HasValue) ? String.Format("{0:#0.0000}", zoneProfit.CostNet) : "0.00",

                Profit = String.Format("{0:#0.00}",(zoneProfit.SaleNet -  zoneProfit.CostNet )) ,

                ProfitPercentage = (zoneProfit.SaleNet.HasValue) ? String.Format("{0:#,##0.00%}",(1 -  zoneProfit.CostNet / zoneProfit.SaleNet )  ): "-100%",

                
            };
        }

        public static List<ZoneProfitFormated> MapZoneProfits(List<ZoneProfit> zoneProfits)
        {
            List<ZoneProfitFormated> models = new List<ZoneProfitFormated>();
            if (zoneProfits != null)
                foreach (var z in zoneProfits)
                {
                    models.Add(MapZoneProfit(z));
                }
            return models;
        }

        public static string getFormatedString(int num)
        {
            string s = "0.0" ;
            for (var i = 0; i < num -1; i++)
            {
                s += "0";
            }

            return s;
        }


    }
}
