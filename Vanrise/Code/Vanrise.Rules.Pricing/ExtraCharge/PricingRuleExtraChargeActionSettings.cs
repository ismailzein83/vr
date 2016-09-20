using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleExtraChargeActionSettings
    {
        public virtual Guid ConfigId { get; set; }

        internal protected abstract void Execute(IPricingRuleExtraChargeActionContext context);

        public abstract string GetDescription();
    }
}
