using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum PricingRuleType { Tariff = 0, TOD = 1, ExtraCharge = 2 }

    public abstract class PricingRuleSettings
    {
        public PricingRuleType RuleType { get; set; }
    }
}
