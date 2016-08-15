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
    public class OtherRateEEDGreaterThanValidEEDCondition : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            return String.Format("OtherRate.EED > Zone.ValidEED");
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            List<DateTime?> endEffectiveDates;

            if (zone.OtherRatesToChange != null)
            {
                foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
                {
                    if (zone.CurrentRate != null && zone.CurrentRate.Rate != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.CurrentRate.Rate.EED };

                        if (otherRateToChange.EED < endEffectiveDates.VRMinimumDate())
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.NormalRateToChange.EED };

                        if (otherRateToChange.EED > endEffectiveDates.VRMinimumDate())
                            return false;
                    }
                }
            }

            if (zone.OtherRatesToClose != null)
            {
                foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
                {
                    if (zone.CurrentRate != null && zone.CurrentRate.Rate != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.CurrentRate.Rate.EED };

                        if (otherRateToClose.CloseEffectiveDate < endEffectiveDates.VRMinimumDate())
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.NormalRateToChange.EED };

                        if (otherRateToClose.CloseEffectiveDate > endEffectiveDates.VRMinimumDate())
                            return false;
                    }
                }
            }

            return true;
        }
    }
}
