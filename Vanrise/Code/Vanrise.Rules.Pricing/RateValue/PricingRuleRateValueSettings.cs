using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleRateValueSettings
    {
        public int ConfigId { get; set; }

        protected abstract void Execute(IPricingRuleRateValueContext context);

        public void ApplyRateValueRule(IPricingRuleRateValueContext context)
        {
            this.Execute(context);
        }
    }
}
