using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleRateTypeItemSettings
    {
        public virtual Guid ConfigId { get; set; }

        public int RateTypeId { get; set; }

        internal protected abstract bool Evaluate(IPricingRuleRateTypeItemContext context);

        public abstract string GetDescription();
    }
}
