using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public interface IPricingRuleExtraChargeActionContext
    {
        DateTime? TargetTime { get; }

        Decimal Rate { get; set; }
    }
}
