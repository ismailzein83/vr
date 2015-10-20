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
       
        public T GetMatchRule(PricingRuleTarget target)
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

        protected abstract PricingRuleTODTarget CreateTODTarget(R input);
        protected abstract PricingRuleTariffTarget CreateTariffTarget(R input);
        protected abstract PricingRuleExtraChargeTarget CreateExtraChargeTarget(R input);

        public PricingRulesResult ApplyPricingRules(R input)
        {
            PricingRulesResult result = new PricingRulesResult();

            PricingRuleTODTarget todTarget = CreateTODTarget(input);
            todTarget.Rate = input.Rate;
            todTarget.EffectiveOn = input.EffectiveOn;
            todTarget.IsEffectiveInFuture = input.IsEffectiveInFuture;


            var todPricingRule = GetMatchRule(todTarget);
            if (todPricingRule != null)
            {
                PricingRuleTODContext todContext = new PricingRuleTODContext();
                (todPricingRule.Settings as PricingRuleTODSettings).Execute(todContext, todTarget);
                result.Rate = todTarget.RateValueToUse;
            }
            else
                result.Rate = input.Rate.NormalRate;

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

            PricingRuleTariffTarget tariffTarget = CreateTariffTarget(input);
            tariffTarget.Rate = result.Rate;
            tariffTarget.EffectiveOn = input.EffectiveOn;
            tariffTarget.IsEffectiveInFuture = input.IsEffectiveInFuture;

            var tariffPricingRule = GetMatchRule(tariffTarget);
            if (tariffPricingRule != null)
            {
                PricingRuleTariffContext tariffContext = new PricingRuleTariffContext();
                (tariffPricingRule.Settings as PricingRuleTariffSettings).Execute(tariffContext, tariffTarget);
                result.Rate = tariffTarget.Rate;
                result.TotalAmount = tariffTarget.TotalAmount;
            }
            return result;
        }
    }
}
