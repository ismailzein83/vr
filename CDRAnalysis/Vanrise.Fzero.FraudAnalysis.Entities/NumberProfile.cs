using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class NumberProfile
    {
        public Dictionary<String, Decimal> AggregateValues = new Dictionary<String, Decimal>();
        public String SubscriberNumber { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? IsOnNet { get; set; }
        public int? PeriodId { get; set; } 
    }
}
