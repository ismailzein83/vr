using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryView
    {
        public int? ProfileID { get; set; }
        public string ProfileName { get; set; }
        public string CarrierID { get; set; }
        public string CarrierName { get; set; }
        public int Attempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public decimal DurationsInMinutes { get; set; }
        public decimal ASR { get; set; }
        public decimal ACD { get; set; }
        public decimal DeliveredASR { get; set; }
        public decimal AveragePDD { get; set; }
        public int? NumberOfCalls { get; set; }
        public decimal? PricedDuration { get; set; }
        public decimal SaleNets { get; set; }
        public decimal CostNets { get; set; }
        public decimal Profit { get; set; }

    }
}
