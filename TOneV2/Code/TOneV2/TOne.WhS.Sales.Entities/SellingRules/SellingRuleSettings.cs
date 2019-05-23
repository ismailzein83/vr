using System;
using System.Collections.Generic;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRuleSettings
    {
        public RateRuleGrouped RateRuleGrouped { get; set; }

        public void ApplySellingRule(ISellingRuleContext context)
        {
            if (RateRuleGrouped == null)
                return;

            foreach (var rateRule in RateRuleGrouped.RateRules)
            {
                rateRule.Threshold.Execute(context);
            }
        }
    }
}
