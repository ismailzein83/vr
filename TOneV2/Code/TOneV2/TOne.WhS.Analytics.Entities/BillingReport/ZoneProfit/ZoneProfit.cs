using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities.BillingReport
{
    public class ZoneProfit
    {
        public long SaleZoneID { get; set; }
        public long SupplierZoneID { get; set; }
        public int SupplierID { get; set; }
        public int CustomerId { get; set; }
        public int Calls { get; set; }
        public decimal? DurationNet { get; set; }
        public decimal? CostDuration { get; set; }
        public decimal? SaleDuration { get; set; }
        public double? CostNet { get; set; }
        public double? SaleNet { get; set; }
    }
}
