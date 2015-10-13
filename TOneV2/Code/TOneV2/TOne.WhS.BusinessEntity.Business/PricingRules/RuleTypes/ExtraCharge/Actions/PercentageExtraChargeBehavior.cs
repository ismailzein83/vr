using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities.PricingRules.RuleTypes.ExtraCharge.Actions;

namespace TOne.WhS.BusinessEntity.Business.PricingRules.RuleTypes.ExtraCharge.Actions
{
    public class PercentageExtraChargeBehavior : Entities.PricingRuleExtraChargeActionBehavior
    {
        public override void Execute(Entities.PricingRuleExtraChargeActionSettings settings, Entities.PricingRuleExtraChargeTarget target)
        {
            PercentageExtraChargeSettings percentageExtraChargeSettings = settings as PercentageExtraChargeSettings;
            if (target.Rate >= percentageExtraChargeSettings.FromRate && target.Rate < percentageExtraChargeSettings.ToRate)
                target.Rate += percentageExtraChargeSettings.ExtraPercentage * target.Rate / 100;
        }
    }
}
