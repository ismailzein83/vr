using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.CDRImport.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public class DWFact
    {
        public string IMEI { get; set; }
        public string MSISDN { get; set; }
        public int? CaseId { get; set; }
        public decimal? DurationInSeconds { get; set; }
        public int? CallClassId { get; set; }
        public CallType CallType { get; set; }
        public CaseStatus? CaseStatus { get; set; }
        public NetType? NetworkType { get; set; }
        public PeriodEnum? Period { get; set; }
        public int? StrategyId { get; set; }
        public StrategyKind? StrategyKind { get; set; }
        public SubscriberType? SubscriberType { get; set; }
        public SuspicionLevel? SuspicionLevel { get; set; }
        public DateTime ConnectDateTime { get; set; }
        public DateTime? CaseGenerationTime { get; set; }
        public int? StrategyUserId { get; set; }
        public int? CaseUserId { get; set; }
        public int? BTSId { get; set; }

    }
}
