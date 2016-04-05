using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDRComparison.Entities
{
    public class CDRComparisonSummary
    {
        public int AllCDRsCount { get; set; }
        public int SystemMissingCDRsCount { get; set; }
        public int PartnerMissingCDRsCount { get; set; }
        public int PartialMatchCDRsCount { get; set; }
        public int DisputeCDRsCount { get; set; }
    }
}
