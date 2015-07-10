using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class RateLoss
    {
        public string CostZone { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public double? SaleRate { get; set; }
        public double? CostRate { get; set; }
        public decimal? CostDuration { get; set; }
        public decimal? SaleDuration { get; set; }
        public double? CostNet { get; set; }
        public double? SaleNet { get; set; }
        public int SaleZoneID { get; set; }
        public string CarrierGroupsNames { get; set; }
    }
}
