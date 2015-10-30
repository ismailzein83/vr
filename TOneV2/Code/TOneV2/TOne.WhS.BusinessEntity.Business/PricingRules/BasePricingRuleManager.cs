using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class BasePricingRuleManager<T, Q, R> : Vanrise.Rules.RuleManager<T,Q>
        where T : BasePricingRule
        where Q : class
        where R : PricingRulesInput
    {
       
        protected T GetMatchRule(PricingRuleTarget target)
        {
            var ruleTree = GetRuleTree(target.RuleType);
            if (ruleTree == null)
                return null;
            else
                return ruleTree.GetMatchRule(target) as T;
        }

        Vanrise.Rules.RuleTree GetRuleTree(PricingRuleType ruleType)
        {
            return GetCachedOrCreate(String.Format("GetRuleTree_{0}", ruleType),
                () =>
                {
                    var rules = GetFilteredRules(rule => rule.Settings.RuleType == ruleType);
                    return new Vanrise.Rules.RuleTree(rules, GetBehaviors());
                });
        }

        protected abstract IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors();

        protected abstract PricingRuleRateTypeTarget CreateRateTypeTarget(R input);
        protected abstract PricingRuleTariffTarget CreateTariffTarget(R input);
        protected abstract PricingRuleExtraChargeTarget CreateExtraChargeTarget(R input);

        public PricingRulesResult ApplyPricingRules(R input)
        {
            PricingRulesResult result = new PricingRulesResult();

            ApplyRateTypeRule(input, result);

            ApplyExtraChargeRule(input, result);

            ApplyTariffRule(input, result);

            return result;
        }

        private void ApplyRateTypeRule(R input, PricingRulesResult result)
        {
            if (input.Rate.OtherRates != null && input.Rate.OtherRates.Count > 0)
            {
                bool isRateFound = false;
                PricingRuleRateTypeTarget rateTypeTarget = CreateRateTypeTarget(input);
                rateTypeTarget.EffectiveOn = input.EffectiveOn;
                rateTypeTarget.IsEffectiveInFuture = input.IsEffectiveInFuture;

                var rateTypePricingRule = GetMatchRule(rateTypeTarget);
                if (rateTypePricingRule != null)
                {
                    PricingRuleRateTypeItemContext rateTypeContext = new PricingRuleRateTypeItemContext();
                    foreach (var rateTypeItem in (rateTypePricingRule.Settings as PricingRuleRateTypeSettings).Items)
                    {
                        if (rateTypeItem.Evaluate(rateTypeContext, rateTypeTarget))
                        {
                            Decimal rateToUse;
                            if (input.Rate.OtherRates.TryGetValue(rateTypeItem.RateTypeId, out rateToUse))
                            {
                                isRateFound = true;
                                result.Rate = rateToUse;
                                break;
                            }
                        }
                    }
                }
                if (!isRateFound)
                    result.Rate = input.Rate.NormalRate;
            }
            else
                result.Rate = input.Rate.NormalRate;
        }

        private void ApplyExtraChargeRule(R input, PricingRulesResult result)
        {
            PricingRuleExtraChargeTarget extraChargeTarget = CreateExtraChargeTarget(input);
            extraChargeTarget.Rate = result.Rate;
            extraChargeTarget.EffectiveOn = input.EffectiveOn;
            extraChargeTarget.IsEffectiveInFuture = input.IsEffectiveInFuture;

            var extraChargePricingRule = GetMatchRule(extraChargeTarget);
            if (extraChargePricingRule != null)
            {
                PricingRuleExtraChargeActionContext extraChargeContext = new PricingRuleExtraChargeActionContext();
                foreach (var action in (extraChargePricingRule.Settings as PricingRuleExtraChargeSettings).Actions)
                {
                    action.Execute(extraChargeContext, extraChargeTarget);
                }
                result.Rate = extraChargeTarget.Rate;
            }
        }

        private void ApplyTariffRule(R input, PricingRulesResult result)
        {
            PricingRuleTariffTarget tariffTarget = CreateTariffTarget(input);
            tariffTarget.Rate = result.Rate;
            tariffTarget.EffectiveOn = input.EffectiveOn;
            tariffTarget.IsEffectiveInFuture = input.IsEffectiveInFuture;

            var tariffPricingRule = GetMatchRule(tariffTarget);
            if (tariffPricingRule != null)
            {
                PricingRuleTariffContext tariffContext = new PricingRuleTariffContext();
                (tariffPricingRule.Settings as PricingRuleTariffSettings).Execute(tariffContext, tariffTarget);
                result.Rate = tariffTarget.EffectiveRate;
                result.TotalAmount = tariffTarget.TotalAmount;
            }
        }

    }
}
