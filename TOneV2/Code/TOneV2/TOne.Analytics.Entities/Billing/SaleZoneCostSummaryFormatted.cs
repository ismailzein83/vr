using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleZoneCostSummaryFormatted
    {
        public double? AvgCost { get; set; }
        public string AvgCostFormatted { get; set; }
        public int salezoneID { get; set; }
        public string salezoneIDFormatted { get; set; }
        public decimal? AvgDuration { get; set; }
        public string AvgDurationFormatted { get; set; }
    }
}
