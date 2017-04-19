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

        public int? DestinationCurrencyId { get; set; }

        public int SourceCurrencyId { get; set; }

        public bool IsTaxApplied { get; set; }
    }
}
