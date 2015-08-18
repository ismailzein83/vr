using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryStats
    {
        public string SupplierID { get; set; }
        public decimal Attempts { get; set; }
        public decimal? SuccessfulAttempts { get; set; }
        public decimal? DurationsInMinutes { get; set; }
        public decimal? ASR { get; set; }
        public decimal? ACD { get; set; }
        public decimal? DeliveredASR { get; set; }
        public decimal? AveragePDD { get; set; }
        public decimal? NumberOfCalls { get; set; }
        public decimal? PricedDuration { get; set; }
        public decimal? Sale_Nets { get; set; }
        public decimal? Cost_Nets { get; set; }
        public decimal? Profit { get; set; }
        public double? Percentage { get; set; }
        public int rownIndex { get; set; }
    }
}