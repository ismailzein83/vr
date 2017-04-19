using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleTaxSettings
    {
        public List<PricingRuleTaxActionSettings> Actions { get; set; }

        public int CurrencyId { get; set; }

        public void ApplyTaxRule(IPricingRuleTaxContext context)
        {
            PricingRuleTaxActionContext actionContext = new PricingRuleTaxActionContext
            {
                Amount = context.Amount,
                TargetTime = context.TargetTime,
                DestinationCurrencyId = context.DestinationCurrencyId,
                SourceCurrencyId = context.SourceCurrencyId
            };

            var originalAmount = context.Amount;
            foreach (var action in this.Actions)
            {
                action.Execute(actionContext);
                if (actionContext.IsTaxApplied)
                    break;
            }
            context.TaxAmount = actionContext.Amount - context.Amount;
            context.Amount = actionContext.Amount;
        }
    }
}
