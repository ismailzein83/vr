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

        public override string GetSettingsDescription(GenericRuleDefinitionSettings settingsDefinition)
        {
            return Settings.GetDescription();
        }
    }
}
