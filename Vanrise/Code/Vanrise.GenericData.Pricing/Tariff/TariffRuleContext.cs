using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Rules;
using Vanrise.Rules.Pricing;

namespace Vanrise.GenericData.Pricing
{
    public class TariffRuleContext : IPricingRuleTariffContext
    {
        public DateTime? TargetTime { get; set; }
         
        public Decimal Rate { get; set; }
         
        public Decimal? DurationInSeconds { get; set; }
         
        public Decimal EffectiveRate { get; set; }
         
        public Decimal? EffectiveDurationInSeconds { get; set; }
         
        public Decimal? TotalAmount { get; set; }

        public IVRRule Rule { get; set; }

        public Decimal ExtraChargeRate { get; set; }

        public Decimal ExtraChargeValue { get; set; }

        public int? DestinationCurrencyId { get; set; }

        public int SourceCurrencyId { get; set; }
    }
}
