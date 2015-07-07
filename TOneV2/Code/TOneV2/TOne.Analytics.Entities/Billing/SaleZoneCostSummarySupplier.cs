using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleZoneCostSummarySupplier
    {
        public string SupplierID { get; set; }
        public double? HighestRate { get; set; }
        public int salezoneID { get; set; }
        public decimal? AvgDuration { get; set; }
    }
}
