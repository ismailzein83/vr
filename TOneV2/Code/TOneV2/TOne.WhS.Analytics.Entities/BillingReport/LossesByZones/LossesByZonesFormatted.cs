using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class LossesByZonesFormatted
    {
       
        public string CostZone { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public decimal? SaleRate { get; set; }
        public string SaleRateFormatted { get; set; }
        public decimal? CostRate { get; set; }
        public string CostRateFormatted { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public decimal? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public decimal? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public int SaleZoneID { get; set; }
        public decimal Loss { get; set; }
        public string LossFormatted { get; set; }
        public string LossPerFormatted { get; set; }
        public string CarrierGroupsNames { get; set; }


        /// <summary>
        /// DO NOT REMOVE
        /// the purpose of this method is to set the schema in the RDLC file
        /// </summary>
        /// <returns></returns>
        /// 
        public LossesByZonesFormatted() { }
        public IEnumerable<LossesByZonesFormatted> GetRateLossFormattedRDLCSchema()
        {
            return null;
        }
    }
}