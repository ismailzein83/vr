using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleZoneCostSummary
    {
        public double? AvgCost { get; set; }
        public int salezoneID { get; set; }
        public decimal? AvgDuration { get; set; }
    }
}
