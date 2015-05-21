using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class ZoneProfit
    {

        public string CostZone { get; set; }
        public string SaleZone { get; set; }
        public string SupplierID { get; set; }
        public string CustomerID { get; set; }
        public int Calls { get; set; }
        public decimal? DurationNet { get; set; }
        public decimal? CostDuration { get; set; }
        public decimal? SaleDuration { get; set; }
        public double? CostNet { get; set; }
        public double? SaleNet { get; set; }
    }
}
