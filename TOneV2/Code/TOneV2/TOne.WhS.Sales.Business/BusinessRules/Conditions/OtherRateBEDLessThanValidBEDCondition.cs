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
			string zoneBED = UtilitiesManager.GetDateTimeAsString(zone.BED);

			var messageBuilder = new StringBuilder(string.Format("BED of the Other Rate of Zone '{0}' must be greater than", zone.ZoneName));

			if (zone.SoldOn.HasValue)
			{
				string soldOn = UtilitiesManager.GetDateTimeAsString(zone.SoldOn.Value);
				messageBuilder.Append(string.Format(" the maximum between the BED '{0}' of the Zone and the date '{1}' when the Country was sold", zoneBED, soldOn));
			}
			else
			{
				messageBuilder.Append(string.Format(" the BED '{0}' of the Zone", zoneBED));
			}

			return messageBuilder.ToString();
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

                    if (zone.ZoneRateGroup != null && zone.ZoneRateGroup.NormalRate != null)
                    {
                        beginEffectiveDates = new List<DateTime?>() { zone.BED, zone.SoldOn, zone.ZoneRateGroup.NormalRate.BED };

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
