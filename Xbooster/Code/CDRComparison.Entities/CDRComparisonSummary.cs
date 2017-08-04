using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDRComparisonSummary
    {
        public int SystemCDRsCount { get; set; }
        public int PartnerCDRsCount { get; set; }
        public int SystemMissingCDRsCount { get; set; }
        public int PartnerMissingCDRsCount { get; set; }
        public int PartialMatchCDRsCount { get; set; }
        public int DisputeCDRsCount { get; set; }
        public decimal DurationOfSystemCDRs { get; set; }
        public decimal DurationOfPartnerCDRs { get; set; }
        public decimal DurationOfSystemMissingCDRs { get; set; }
        public decimal DurationOfPartnerMissingCDRs { get; set; }
        public decimal DurationOfSystemPartialMatchCDRs { get; set; }
        public decimal DurationOfPartnerPartialMatchCDRs { get; set; }
        public decimal TotalDurationDifferenceOfPartialMatchCDRs { get; set; }
        public decimal DurationOfSystemDisputeCDRs { get; set; }
        public decimal DurationOfPartnerDisputeCDRs { get; set; }
        public int SystemInvalidCDRsCount { get; set; }
        public int PartnerInvalidCDRsCount { get; set; }
        public decimal SystemInvalidCDRsDuration { get; set; }
        public decimal PartnerInvalidCDRsDuration { get; set; }
    }
}
