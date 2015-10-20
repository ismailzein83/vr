using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingRuleTariffTarget : PricingRuleTarget
    {
        public IRate Rate { get; set; }

        public Decimal EffectiveRateValue { get; set; }

        public Decimal TotalAmount { get; set; }

        public override PricingRuleType RuleType
        {
            get { return PricingRuleType.Tariff; }
        }
    }
}
