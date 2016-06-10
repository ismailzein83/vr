using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleRateTypeContext : IRuleExecutionContext
    {
        Decimal NormalRate { get; }

        Dictionary<int, Decimal> RatesByRateType { get; }

        DateTime? TargetTime { get; }

        Decimal EffectiveRate { set; }

        int? RateTypeId { set; }
    }
}
