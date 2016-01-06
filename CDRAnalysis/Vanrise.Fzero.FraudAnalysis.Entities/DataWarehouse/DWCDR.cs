using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DWCDR
    {
        public int? CDRId { get; set; }
        public string IMEI { get; set; }
        public string MSISDN { get; set; }
        public int? CaseId { get; set; }
        public decimal? Duration { get; set; }
        public int? CallClass { get; set; }
        public int? CallType { get; set; }
        public int? CaseStatus { get; set; }
        public int? NetworkType { get; set; }
        public int? Period { get; set; }
        public int? Strategy { get; set; }
        public int? StrategyKind { get; set; }
        public string SubscriberType { get; set; }
        public int? SuspicionLevel { get; set; }
        public DateTime? ConnectTime { get; set; }
        public int? StrategyUser { get; set; }
        public int? CaseUser { get; set; }
        public int? BTS { get; set; }
    }
}
