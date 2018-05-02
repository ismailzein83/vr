using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class TariffRule : Vanrise.GenericData.Entities.GenericRule
    {
        public Vanrise.Rules.Pricing.PricingRuleTariffSettings Settings { get; set; }

        public override string GetSettingsDescription(IGenericRuleSettingsDescriptionContext context)
        {
            return Settings.GetDescription();
        }

        public override bool AreSettingsMatched(object ruleDefinitionSettings, object settingsFilterValue)
        {
            return true;
        }
        public override Dictionary<string, object> GetSettingsValuesByName()
        {
            return Settings.GetSettingsValuesByName();
        }
    }
}
