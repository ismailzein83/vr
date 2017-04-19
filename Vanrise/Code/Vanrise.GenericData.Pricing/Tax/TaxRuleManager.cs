using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class TaxRuleManager : Vanrise.GenericData.Business.GenericRuleManager<TaxRule>
    {
        public void ApplyTaxRule(IPricingRuleTaxContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var taxPricingRule = GetMatchRule(ruleDefinitionId, target);
            if (taxPricingRule != null)
            {
                context.SourceCurrencyId = taxPricingRule.Settings.CurrencyId;
                taxPricingRule.Settings.ApplyTaxRule(context);
                context.Rule = taxPricingRule;
            }
        }
    }
}
