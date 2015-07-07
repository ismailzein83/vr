using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class SaleZoneCostSummarySupplierFormatted
    {
        public string SupplierID { get; set; }

        public double? HighestRate { get; set; }
        public string HighestRateFormatted { get; set; }

        public int salezoneID { get; set; }
        public string salezoneIDFormatted { get; set; }
        
        public decimal? AvgDuration { get; set; }
        public string AvgDurationFormatted { get; set; }
    }
}
