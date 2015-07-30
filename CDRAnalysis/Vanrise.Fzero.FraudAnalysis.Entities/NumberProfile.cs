using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public NumberProfile()
        {
            AggregateValues = new Dictionary<string, decimal>();
        }

        public Dictionary<String, Decimal> AggregateValues { get; set; }
        public String SubscriberNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsOnNet { get; set; }
        public int PeriodId { get; set; }

        public int StrategyId { get; set; } 


    }
}
