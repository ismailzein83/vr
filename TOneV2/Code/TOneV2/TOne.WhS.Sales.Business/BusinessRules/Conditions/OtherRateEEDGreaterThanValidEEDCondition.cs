using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.BusinessProcess.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class OtherRateEEDGreaterThanValidEEDCondition : BusinessRuleCondition
    {
        public override string GetMessage(IRuleTarget target)
        {
            DataByZone zone = (target as DataByZone);
            return String.Format("EED of the other rate of zone {0} must be less than or equal to EED ({1}) of the zone", zone.ZoneName, zone.EED);
        }

        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as DataByZone != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zone = context.Target as DataByZone;

            List<DateTime?> endEffectiveDates;

            if (zone.NormalRateToClose != null && zone.ZoneRateGroup != null && zone.ZoneRateGroup.OtherRatesByType != null)
            {
                IEnumerable<ZoneRate> otherRates = zone.ZoneRateGroup.OtherRatesByType.Values;

                foreach (ZoneRate otherRate in otherRates)
                {
                    bool isOtherRateChangedOrClosed =
                        (zone.OtherRatesToChange != null && zone.OtherRatesToChange.Any(x => x.RateTypeId.Value == otherRate.RateTypeId.Value)) ||
                        (zone.OtherRatesToClose != null && zone.OtherRatesToClose.Any(x => x.RateTypeId.Value == otherRate.RateTypeId.Value));

                    if (isOtherRateChangedOrClosed)
                        continue; // Changed and closed other rates are validated below

                    if (!otherRate.EED.HasValue || otherRate.EED.Value > zone.NormalRateToClose.CloseEffectiveDate)
                        return false;
                }
            }

            if (zone.OtherRatesToChange != null)
            {
                foreach (RateToChange otherRateToChange in zone.OtherRatesToChange)
                {
                    if (zone.ZoneRateGroup != null && zone.ZoneRateGroup.NormalRate != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.ZoneRateGroup.NormalRate.EED };
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
                    if (zone.ZoneRateGroup != null && zone.ZoneRateGroup.NormalRate != null)
                    {
                        endEffectiveDates = new List<DateTime?>() { zone.EED, zone.ZoneRateGroup.NormalRate.EED };
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
