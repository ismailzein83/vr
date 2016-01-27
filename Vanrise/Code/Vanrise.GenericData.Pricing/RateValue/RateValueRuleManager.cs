using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleManager : Vanrise.GenericData.Business.GenericRuleManager<RateValueRule>
    {
        public void ApplyRateValueRule(IPricingRuleRateValueContext context, int ruleDefinitionId, GenericRuleTarget target)
        {
            var rateValueRule = GetMatchRule(ruleDefinitionId, target);
            if (rateValueRule != null)
                rateValueRule.Settings.ApplyRateValueRule(context);
        }
    }
}
