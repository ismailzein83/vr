using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class PrepaidTaxRuleManager: GenericRuleManager<PrepaidTaxRule>
    {
        public void ApplyTaxRule(IPrepaidTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyPrepaidTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTaxRule(IPrepaidTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyPrepaidTaxRule(context, () => ruleTree.GetMatchRule(target) as PrepaidTaxRule, target);
        }

        void ApplyPrepaidTaxRule(IPrepaidTaxRuleContext context, Func<PrepaidTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var prepaidTaxRule = getMatchRule();
            if (prepaidTaxRule != null)
            {
                prepaidTaxRule.Settings.ApplyPrepaidTaxRule(context);
                context.Rule = prepaidTaxRule;
            }
        }
    }
}
