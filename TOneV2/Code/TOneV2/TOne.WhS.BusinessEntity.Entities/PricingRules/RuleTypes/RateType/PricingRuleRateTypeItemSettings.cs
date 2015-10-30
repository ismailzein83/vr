using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleRateTypeItemSettings
    {
        public int ConfigId { get; set; }

        public string RateTypeName { get; set; }

        public abstract bool Evaluate(IPricingRuleRateTypeItemContext context, PricingRuleRateTypeTarget target);
    }
}
