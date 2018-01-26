using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.BP.Arguments
{
    public class RatePlanSubProcessOutput
    {
        public bool ContinueRatePlanProcess { get; set; }
        public List<long> PricelistFileIds { get; set; }
        public bool TerminatedDueBusinessRulesViolation { get; set; }
    }
}
