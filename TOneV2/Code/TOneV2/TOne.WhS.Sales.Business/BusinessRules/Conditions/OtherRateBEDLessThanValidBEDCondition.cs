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
            DataByZone zone = (target as DataByZone);
            return String.Format("BED of the other rate of zone {0} must be greater than or equal to BED ({1}) of the zone", zone.ZoneName, zone.BED);
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
                    if (otherRateToChange.BED == default(DateTime))
                        return false;

                    if (zone.CurrentRate != null && zone.CurrentRate.Rate != null)
                    {
                        beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn, zone.CurrentRate.Rate.BED };

                        if (otherRateToChange.BED >= UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn, zone.NormalRateToChange.BED };

                        if (otherRateToChange.BED >= UtilitiesManager.GetMaxDate(beginEffectiveDates).Value)
                            continue;
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
