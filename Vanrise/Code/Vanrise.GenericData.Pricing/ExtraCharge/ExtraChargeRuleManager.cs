using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class ExtraChargeRuleManager : Vanrise.GenericData.Business.GenericRuleManager<ExtraChargeRule>
    {
        public void ApplyExtraChargeRule(IPricingRuleExtraChargeContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var tariffPricingRule = GetMatchRule(ruleDefinitionId, target);
            if (tariffPricingRule != null)
            {
                context.SourceCurrencyId = tariffPricingRule.Settings.CurrencyId;
                tariffPricingRule.Settings.ApplyExtraChargeRule(context);
                context.Rule = tariffPricingRule;
            }
        }
    }
}
