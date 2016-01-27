﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleExtraChargeSettings
    {
        public List<PricingRuleExtraChargeActionSettings> Actions { get; set; }

        public void ApplyExtraChargeRule(IPricingRuleExtraChargeContext context)
        {
            PricingRuleExtraChargeActionContext actionContext = new PricingRuleExtraChargeActionContext
            {
                Rate = context.Rate,
                TargetTime = context.TargetTime
            };

            var originalRate = context.Rate;
            foreach (var action in this.Actions)
            {
                action.Execute(actionContext);
            }
            context.ExtraChargeValue = actionContext.Rate - context.Rate;
            context.Rate = actionContext.Rate;
        }
    }
}
