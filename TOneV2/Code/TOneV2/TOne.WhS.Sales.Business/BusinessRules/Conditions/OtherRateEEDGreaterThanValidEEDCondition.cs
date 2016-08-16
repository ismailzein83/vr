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
            return String.Format("Cannot add an other rate with EED > valid EED of zone {0}", (target as DataByZone).ZoneName);
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
                        DateTime? validEED = UtilitiesManager.GetMinDate(endEffectiveDates);

                        if (!validEED.HasValue || otherRateToChange.EED <= validEED.Value)
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.NormalRateToChange.EED };
                        DateTime? validEED = UtilitiesManager.GetMinDate(endEffectiveDates);

                        if (!validEED.HasValue || otherRateToChange.EED <= validEED.Value)
                            continue;
                    }

                    return false;
                }
            }

            if (zone.OtherRatesToClose != null)
            {
                foreach (RateToClose otherRateToClose in zone.OtherRatesToClose)
                {
                    if (zone.CurrentRate != null && zone.CurrentRate.Rate != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.CurrentRate.Rate.EED };
                        DateTime? validEED = UtilitiesManager.GetMinDate(endEffectiveDates);

                        if (!validEED.HasValue || otherRateToClose.CloseEffectiveDate <= validEED.Value)
                            continue;
                    }

                    if (zone.NormalRateToChange != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.NormalRateToChange.EED };
                        DateTime? validEED = UtilitiesManager.GetMinDate(endEffectiveDates);

                        if (!validEED.HasValue || otherRateToClose.CloseEffectiveDate <= validEED.Value)
                            continue;
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
