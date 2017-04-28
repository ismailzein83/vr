using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public class PricingRuleTaxActionContext : IPricingRuleTaxActionContext
    {
        public DateTime? TargetTime { get; set; }

        public decimal Amount { get; set; }

        public int RuleCurrencyId { get; set; }

        public int AmountCurrencyId { get; set; }

        public decimal? TaxAmount { get; set; }
    }
}
