using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleTariffContext : IRuleExecutionContext
    {
        DateTime? TargetTime { get; }

        Decimal Rate { get; }

        Decimal? DurationInSeconds { get; }

        Decimal EffectiveRate { set; }

        Decimal? EffectiveDurationInSeconds { set; }

        Decimal? TotalAmount {  set; }
    }
}
