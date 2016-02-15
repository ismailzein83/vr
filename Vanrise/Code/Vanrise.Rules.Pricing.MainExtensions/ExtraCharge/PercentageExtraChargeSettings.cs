using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.ExtraCharge
{
    public class PercentageExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraPercentage { get; set; }

        protected override void Execute(IPricingRuleExtraChargeActionContext context)
        {
            if (context.Rate >= this.FromRate && context.Rate < this.ToRate)
                context.Rate += Math.Ceiling(this.ExtraPercentage * context.Rate / 100);
        }

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();
            description.Append(String.Format("From Rate: {0}; ", FromRate));
            description.Append(String.Format("To Rate: {0}; ", ToRate));
            description.Append(String.Format("Extra Percentage: {0}", ExtraPercentage));
            return description.ToString();
        }
    }
}
