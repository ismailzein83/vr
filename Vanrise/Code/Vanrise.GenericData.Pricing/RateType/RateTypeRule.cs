using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRule : Vanrise.GenericData.Entities.GenericRule
    {
        public Vanrise.Rules.Pricing.PricingRuleRateTypeSettings Settings { get; set; }

        public override string GetSettingsDescription(GenericRuleDefinitionSettings settingsDefinition)
        {
            if (Settings != null && Settings.Items != null)
            {
                List<string> itemDescriptions = new List<string>();
                foreach (var item in Settings.Items)
                    itemDescriptions.Add(item.GetDescription());
                return String.Join("; ", itemDescriptions);
            }
            return null;
        }
    }
}
