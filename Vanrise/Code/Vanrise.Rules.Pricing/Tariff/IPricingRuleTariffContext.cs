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

        /// <summary>
        /// This rate is calculated for 1 minute
        /// </summary>
        Decimal EffectiveRate { set; }

        Decimal? EffectiveDurationInSeconds { set; }

        Decimal? TotalAmount { set; }

        Decimal? ExtraChargeRate { get; }

        Decimal? ExtraChargeValue { set; }

        int? DestinationCurrencyId { get; }

        int SourceCurrencyId { get; set; }
    }
}
