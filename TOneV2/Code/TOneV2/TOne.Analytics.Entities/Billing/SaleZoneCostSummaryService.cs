using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleZoneCostSummaryService
    {
        public double? AvgServiceCost { get; set; }
        public int salezoneID { get; set; }
        public string Service { get; set; }
        public decimal? AvgDuration { get; set; }
    }
}
