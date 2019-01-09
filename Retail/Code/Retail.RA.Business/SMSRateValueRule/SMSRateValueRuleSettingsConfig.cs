using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleSMSRateValueSettingsConfig : ExtensionConfiguration
    {
        public const string EXTENSION_TYPE = "RA_Rules_SMSRateValueSettings";
        public string Editor { get; set; }
    }
}
