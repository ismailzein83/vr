using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions
{
    public class PercentageExtraChargeSettings : PricingRuleExtraChargeActionSettings
    {
        public Decimal FromRate { get; set; }

        public Decimal ToRate { get; set; }

        public Decimal ExtraPercentage { get; set; }

        public override void Execute(IPricingRuleExtraChargeActionContext context, PricingRuleExtraChargeTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
