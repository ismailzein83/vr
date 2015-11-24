using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions
{
    public class PercentageExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraPercentage { get; set; }

        public override void Execute(IPricingRuleExtraChargeActionContext context, PricingRuleExtraChargeTarget target)
        {
            if (target.Rate >= this.FromRate && target.Rate < this.ToRate)
                target.Rate += Math.Ceiling(this.ExtraPercentage * target.Rate / 100);
        }
    }
}
