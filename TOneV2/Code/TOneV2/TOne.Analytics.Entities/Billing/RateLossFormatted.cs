using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RateLossFormatted
    {
        public string CostZone { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string Supplier { get; set; }
        public string CustomerID { get; set; }
        public string Customer { get; set; }
        public double? SaleRate { get; set; }
        public string SaleRateFormatted { get; set; }
        public double? CostRate { get; set; }
        public string CostRateFormatted { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public int SaleZoneID { get; set; }
        public double Loss { get; set; }
        public string LossFormatted { get; set; }
        public string LossPerFormatted { get; set; }
        public string CarrierGroupsNames { get; set; }
    }
}