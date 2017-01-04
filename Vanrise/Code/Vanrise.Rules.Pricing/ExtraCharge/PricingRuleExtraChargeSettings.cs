using System.Collections.Generic;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleExtraChargeSettings
    {
        public List<PricingRuleExtraChargeActionSettings> Actions { get; set; }

        public int CurrencyId { get; set; }

        public void ApplyExtraChargeRule(IPricingRuleExtraChargeContext context)
        {
            PricingRuleExtraChargeActionContext actionContext = new PricingRuleExtraChargeActionContext
            {
                Rate = context.Rate,
                TargetTime = context.TargetTime,
                DestinationCurrencyId = context.DestinationCurrencyId,
                SourceCurrencyId = context.SourceCurrencyId
            };

            var originalRate = context.Rate;
            foreach (var action in this.Actions)
            {
                action.Execute(actionContext);
                if (actionContext.IsExtraChargeApplied)
                    break;
            }
            context.ExtraChargeRate = actionContext.Rate - context.Rate;
            context.Rate = actionContext.Rate;
        }
    }
}