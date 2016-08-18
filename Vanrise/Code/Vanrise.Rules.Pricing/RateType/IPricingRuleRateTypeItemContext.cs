using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleRateTypeItemContext
    {
        List<int> RateTypes { get; }

        DateTime? TargetTime { get; }
    }
}
