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
    public class TariffRuleManager : Vanrise.GenericData.Business.GenericRuleManager<TariffRule>
    {
        public void ApplyTariffRule(IPricingRuleTariffContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyTariffRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTariffRule(IPricingRuleTariffContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyTariffRule(context, () => ruleTree.GetMatchRule(target) as TariffRule, target);
        }

        void ApplyTariffRule(IPricingRuleTariffContext context, Func<TariffRule> getMatchRule, GenericRuleTarget target)
        {
            var tariffPricingRule = getMatchRule();

            if (tariffPricingRule == null)
                throw new NullReferenceException("tariffPricingRule");

            context.SourceCurrencyId = tariffPricingRule.Settings.CurrencyId;
            tariffPricingRule.Settings.ApplyTariffRule(context);
            context.Rule = tariffPricingRule;
            //else
            //{
            //    var effectiveRate = context.Rate;
            //    context.EffectiveRate = effectiveRate;
            //    if (context.DurationInSeconds != null)
            //    {
            //        context.EffectiveDurationInSeconds = context.DurationInSeconds;
            //        context.TotalAmount = effectiveRate * (context.DurationInSeconds.Value / 60);
            //    }
            //}
        }
    }
}
