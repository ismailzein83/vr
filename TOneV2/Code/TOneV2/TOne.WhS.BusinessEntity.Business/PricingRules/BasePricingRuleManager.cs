using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public abstract class BasePricingRuleManager<T, Q> : Vanrise.Rules.RuleManager<T,Q>
        where T : BasePricingRule
        where Q : class
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
    }
}
