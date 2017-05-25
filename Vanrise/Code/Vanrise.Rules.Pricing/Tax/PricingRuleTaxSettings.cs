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
                RuleCurrencyId = this.CurrencyId,
                AmountCurrencyId = context.CurrencyId
               
            };
            
            foreach (var action in this.Actions)
            {
                action.Execute(actionContext);
                if (actionContext.TaxAmount.HasValue)
                {
                    context.TaxAmount = actionContext.TaxAmount.Value;
                    context.Percentage = actionContext.Percentage;
                    break;
                }
            }
        }
    }
}
