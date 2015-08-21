using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryStats
    {
        public string GroupID { get; set; }
        public double? Attempts { get; set; }
        public double? SuccessfulAttempts { get; set; }
        public double? DurationsInMinutes { get; set; }
        public double? ASR { get; set; }
        public double? ACD { get; set; }
        public double? DeliveredASR { get; set; }
        public double? AveragePDD { get; set; }
        public double? NumberOfCalls { get; set; }
        public double? PricedDuration { get; set; }
        public double? Sale_Nets { get; set; }
        public double? Cost_Nets { get; set; }
        public double? Profit { get; set; }
        public double? Percentage { get; set; }
        public int rownIndex { get; set; }
        public decimal? ProfitPercentage { get; set; }
        public string GroupName { get; set; }
    }
}