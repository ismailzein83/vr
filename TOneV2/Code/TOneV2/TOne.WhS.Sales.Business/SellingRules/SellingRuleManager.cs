using System;
using Vanrise.Common;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SellingRuleManager : Vanrise.GenericData.Business.GenericRuleManager<SellingRule>
    {
        public void ApplySellingRule(ISellingRuleContext context, Guid ruleDefinitionId, GenericRuleTarget target)
        {
            var sellingRule = GetMatchRule(ruleDefinitionId, target);

            if (sellingRule == null)
                return;

            sellingRule.Settings.ThrowIfNull("sellingRule.Settings", sellingRule.RuleId);

            sellingRule.Settings.ApplySellingRule(context);
            context.Rule = sellingRule;
        }

        public IEnumerable<SellingRuleThresholdSettings> GetSellingRuleThresholdTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SellingRuleThresholdSettings>(SellingRuleThresholdSettings.EXTENSION_TYPE);
        }

        public IEnumerable<SellingRuleActionSettings> GetSellingRuleActionTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<SellingRuleActionSettings>(SellingRuleActionSettings.EXTENSION_TYPE);
        }
    }
}
