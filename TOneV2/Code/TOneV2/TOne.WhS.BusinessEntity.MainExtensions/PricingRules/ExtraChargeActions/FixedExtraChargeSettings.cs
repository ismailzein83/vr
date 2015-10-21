using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PricingRules.ExtraChargeActions
{
    public class FixedExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraAmount { get; set; }

        public override void Execute(IPricingRuleExtraChargeActionContext context, PricingRuleExtraChargeTarget target)
        {
            if (target.Rate >= this.FromRate && target.Rate < this.ToRate)
                target.Rate += this.ExtraAmount;
        }
    }
}
