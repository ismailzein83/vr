using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class RateTypeRuleContext : IPricingRuleRateTypeContext
    {
        public Decimal NormalRate { get; set; }

        public Dictionary<int, Decimal> RatesByRateType { get; set; }

        public DateTime? TargetTime { get; set; }

        public Decimal EffectiveRate { get; set; }

        public int? RateTypeId { get; set; }

        public BaseRule Rule { get; set; }
    }
}
