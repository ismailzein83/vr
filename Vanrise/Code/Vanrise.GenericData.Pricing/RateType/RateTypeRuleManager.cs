using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRuleManager : Vanrise.GenericData.Business.GenericRuleManager<RateTypeRule>
    {
        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyRateTypeRule(context, () => base.GetMatchRule(ruleDefinitionId, target), target);
        }

        public IEnumerable<int> GetRateTypes(Guid ruleDefinitionId, GenericRuleTarget target)
        {
            HashSet<int> distinctRateTypes = new HashSet<int>();
            var ruleMatch = base.GetMatchRule(ruleDefinitionId, target);
            if(ruleMatch != null)
            {
                foreach (PricingRuleRateTypeItemSettings item in ruleMatch.Settings.Items)
                {
                    distinctRateTypes.Add(item.RateTypeId);
                }
            }

            return distinctRateTypes;
        }

        public void ApplyRateTypeRule(IPricingRuleRateTypeContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyRateTypeRule(context, () => ruleTree.GetMatchRule(target) as RateTypeRule, target);
        }

        void ApplyRateTypeRule(IPricingRuleRateTypeContext context, Func<RateTypeRule> getMatchRule, GenericRuleTarget target)
        {
            if (!target.IsEffectiveInFuture)
            {
                var rateTypePricingRule = getMatchRule();
                if (rateTypePricingRule != null)
                {
                    rateTypePricingRule.Settings.ApplyRateTypeRule(context);
                    context.Rule = rateTypePricingRule;
                }
            }
        }
    }
}
