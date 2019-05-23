using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class RateRuleGrouped
    {
        public List<RateRule> RateRules { get; set; }
    }
    public class RateRule
    {
        public RateThreshold Threshold { get; set; }
        public RateAction Action { get; set; }
    }
}
