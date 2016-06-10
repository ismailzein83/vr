using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleRateValueContext : IRuleExecutionContext
    {
        Decimal NormalRate { set; }

        Dictionary<int, Decimal> RatesByRateType { set; }
    }
}
