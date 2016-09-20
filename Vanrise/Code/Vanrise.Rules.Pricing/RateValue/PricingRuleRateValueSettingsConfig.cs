using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleRateValueSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "VR_Rules_PricingRuleRateValueSettings";
        public string Editor { get; set; }
    }
}
