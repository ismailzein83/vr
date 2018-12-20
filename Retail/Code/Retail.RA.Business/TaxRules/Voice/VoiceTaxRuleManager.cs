using System;
using Vanrise.GenericData.Entities;
using Vanrise.Rules;

namespace Retail.RA.Business
{
    public class VoiceTaxRuleManager : Vanrise.GenericData.Business.GenericRuleManager<VoiceTaxRule>
    {
        public void ApplyTaxRule(IVoiceTaxRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            this.ApplyVoiceTaxRule(context, () => GetMatchRule(ruleDefinitionId, target), target);
        }

        public void ApplyTaxRule(IVoiceTaxRuleContext context, RuleTree ruleTree, GenericRuleTarget target)
        {
            this.ApplyVoiceTaxRule(context, () => ruleTree.GetMatchRule(target) as VoiceTaxRule, target);
        }

        void ApplyVoiceTaxRule(IVoiceTaxRuleContext context, Func<VoiceTaxRule> getMatchRule, GenericRuleTarget target)
        {
            var voiceTaxRule = getMatchRule();
            if (voiceTaxRule != null)
            {
                voiceTaxRule.Settings.ApplyVoiceTaxRule(context);
                context.Rule = voiceTaxRule;
            }
        }
    }
}