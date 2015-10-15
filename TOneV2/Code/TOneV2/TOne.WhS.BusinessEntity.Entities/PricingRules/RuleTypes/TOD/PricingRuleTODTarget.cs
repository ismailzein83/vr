using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingRuleTODTarget : PricingRuleTargetIdentifier
    {
        public DateTime Time { get; set; }

        public Decimal NormalRate { get; set; }

        public Dictionary<string, Decimal> OtherRates { get; set; }

        public Decimal RateToUse { get; set; }

        public override PricingRuleType RuleType
        {
            get { return PricingRuleType.TOD; }
        }
    }
}
