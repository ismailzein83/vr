using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateValueRuleContext : IPricingRuleRateValueContext
    {
        public int CurrencyId { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> RatesByRateType { set; get; }

        public IVRRule Rule { get; set; }
    }
}
