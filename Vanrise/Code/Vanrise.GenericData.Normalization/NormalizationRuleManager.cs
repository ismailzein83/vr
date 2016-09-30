using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Normalization;

namespace Vanrise.GenericData.Normalization
{
    public class NormalizationRuleManager : GenericRuleManager<NormalizationRule>
    {
        public void ApplyNormalizationRule(INormalizeRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var normalizationPricingRule = GetMatchRule(ruleDefinitionId, target);
            if (normalizationPricingRule != null)
            {
                normalizationPricingRule.Settings.ApplyNormalizationRule(context);
                context.Rule = normalizationPricingRule;
            }
            else
                context.NormalizedValue = context.Value;
        }
    }
}
