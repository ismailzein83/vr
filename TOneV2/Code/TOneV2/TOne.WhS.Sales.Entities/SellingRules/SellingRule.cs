using System;
using System.Text;
using Vanrise.Common;
using Vanrise.GenericData.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class SellingRule : GenericRule
    {
        public SellingRuleSettings Settings { get; set; }
        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            Settings.ThrowIfNull("SellingRuleSettings");
            Settings.RateRuleGrouped.ThrowIfNull("Settings.RateRuleGrouped");
            Settings.RateRuleGrouped.RateRules.ThrowIfNull("RateRules");

            var settingsDescription = new StringBuilder();
            var rateRules = Settings.RateRuleGrouped.RateRules;
            foreach (var rateRule in rateRules)
            {
                settingsDescription.AppendLine(rateRule.Threshold.GetDescription());
            }
            return settingsDescription.ToString();
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }
    }
}
