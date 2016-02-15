﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing.MainExtensions.RateValue
{
    public class FixedRateValueSettings : PricingRuleRateValueSettings
    {
        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> RatesByRateType { get; set; }

        protected override void Execute(IPricingRuleRateValueContext context)
        {
            context.NormalRate = this.NormalRate;
            context.RatesByRateType = this.RatesByRateType;
        }

        public override string GetDescription(GenericRuleDefinitionSettings settingsDefinition)
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("Normal Rate: {0}", NormalRate));
            if (RatesByRateType != null)
            {
                RateTypeManager rateTypeManager = new RateTypeManager();

                description.Append("; Other Rates: ");
                foreach (KeyValuePair<int, Decimal> kvp in RatesByRateType)
                {
                    var rateType = rateTypeManager.GetRateType(kvp.Key);
                    string rateTypeName = (rateType  != null) ? rateType.Name : kvp.Key.ToString();
                    description.Append(String.Format("{0}: {1}; ", rateTypeName, kvp.Value));
                }
            }
            return description.ToString();
        }
    }
}
