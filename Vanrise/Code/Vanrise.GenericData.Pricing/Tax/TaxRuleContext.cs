using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class TaxRuleContext : IPricingRuleTaxContext
    {
        public DateTime? TargetTime { get; set; }

        public decimal Amount { get; set; }

        public decimal TaxAmount { get; set; }
        public Rules.IVRRule Rule { get; set; }
        public int CurrencyId { get; set; }
        public decimal Percentage { get; set; }
    }
}
