using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleTODTarget : PricingRuleTarget
    {
        public DateTime Time { get; set; }

        public IRate Rate { get; set; }

        public Decimal RateValueToUse { get; set; }

        public override PricingRuleType RuleType
        {
            get { return PricingRuleType.TOD; }
        }
    }
}
