using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class OtherRateBEDLessThanValidBEDCondition : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            return String.Format("OtherRate.BED < Zone.ValidBED");
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            if (zone.OtherRatesToChange != null)
            {
                List<DateTime?> beginEffectiveDates;

                foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
                {
                    if (zone.CurrentRate != null && zone.CurrentRate.Rate != null)
                    {
                        beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn, zone.CurrentRate.Rate.BED };

                        if (otherRateToChange.BED >= beginEffectiveDates.VRMaximumDate())
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn, zone.NormalRateToChange.BED };

                        if (otherRateToChange.BED < beginEffectiveDates.VRMaximumDate())
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
