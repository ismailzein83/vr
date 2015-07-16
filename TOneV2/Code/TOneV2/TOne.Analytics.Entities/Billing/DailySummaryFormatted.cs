using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class DailySummaryFormatted
    {
        public string Day { get; set; }
        public int Calls { get; set; }
        public string CallsFormatted { get; set; }
        public decimal? DurationNet { get; set; }
        public string DurationNetFormatted { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double? Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentageFormatted { get; set; }
    }
}
