using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions;

namespace TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.ExtraCharge.Actions
{
    public class FixedExtraChargeBehavior : Entities.PricingRuleExtraChargeActionBehavior
    {
        public override void Execute(Entities.PricingRuleExtraChargeActionSettings settings, Entities.PricingRuleExtraChargeTarget target)
        {
            FixedExtraChargeSettings fixedExtraChargeSettings = settings as FixedExtraChargeSettings;
            if (target.Rate >= fixedExtraChargeSettings.FromRate && target.Rate < fixedExtraChargeSettings.ToRate)
                target.Rate += fixedExtraChargeSettings.ExtraAmount;
        }
    }
}
