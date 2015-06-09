using System;
using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public Dictionary<String, Decimal> AggregateValues = new Dictionary<String, Decimal>();
        public String SubscriberNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsOnNet { get; set; }
        public int PeriodId { get; set; } 
    }
}
