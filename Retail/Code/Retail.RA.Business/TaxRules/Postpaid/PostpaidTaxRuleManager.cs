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
    public class PostpaidTaxRuleManager : GenericRuleManager<PostpaidTaxRule>
    {
        public void ApplyTaxRule(IPostpaidTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyPostpaidTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTaxRule(IPostpaidTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyPostpaidTaxRule(context, () => ruleTree.GetMatchRule(target) as PostpaidTaxRule, target);
        }

        void ApplyPostpaidTaxRule(IPostpaidTaxRuleContext context, Func<PostpaidTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var postpaidTaxRule = getMatchRule();
            if (postpaidTaxRule != null)
            {
                postpaidTaxRule.Settings.ApplyPostpaidTaxRule(context);
                context.Rule = postpaidTaxRule;
            }
        }
    }
}
