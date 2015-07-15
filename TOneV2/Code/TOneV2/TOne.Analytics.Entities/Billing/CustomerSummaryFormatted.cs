using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CustomerSummaryFormatted
    {
        public string Carrier { get; set; }
        public decimal? SaleDuration { get; set; }
        public string SaleDurationFormatted { get; set; }
        public double? SaleNet { get; set; }
        public string SaleNetFormatted { get; set; }
        public decimal? CostDuration { get; set; }
        public string CostDurationFormatted { get; set; }
        public double? CostNet { get; set; }
        public string CostNetFormatted { get; set; }
        public double? Profit { get; set; }
        public string ProfitFormatted { get; set; }
        public string ProfitPercentageFormatted { get; set; }
        public string Customer { get; set; }
        public double Services { get; set; }
        public string ServicesFormatted { get; set; }


    }
}
