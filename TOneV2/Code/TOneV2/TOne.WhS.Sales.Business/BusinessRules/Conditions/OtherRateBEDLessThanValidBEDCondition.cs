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
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target as DataByZone != null;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            var zoneData = context.Target as DataByZone;

            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                DateTime minimumBED = GetMinimumOtherRateBED(zoneData);

                var invalidRateTypeNames = new List<string>();
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    if (otherRateToChange.BED != default(DateTime) && otherRateToChange.BED >= minimumBED)
                        continue;

                    string rateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);

                    if (rateTypeName == null)
                        throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Name of rate type '{0}' was not found", otherRateToChange.RateTypeId.Value));

                    invalidRateTypeNames.Add(rateTypeName);
                }

                if (invalidRateTypeNames.Count > 0)
                {
                    string minimumOtherRateBED = UtilitiesManager.GetDateTimeAsString(minimumBED);
                    string invalidRateTypes = string.Join(",", invalidRateTypeNames);
                    context.Message = string.Format("Other rates of type(s) '{0}' of zone '{1}' must have a BED that's greater than or equal to '{2}'", invalidRateTypes, zoneData.ZoneName, minimumOtherRateBED);
                    return false;
                }
            }

            return true;
        }

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

        #region Private Methods

        private DateTime GetMinimumOtherRateBED(DataByZone zoneData)
        {
            var beginEffectiveDates = new List<DateTime>();

            beginEffectiveDates.Add(zoneData.BED);

            if (zoneData.SoldOn.HasValue)
                beginEffectiveDates.Add(zoneData.SoldOn.Value);

            ZoneRate currentRate = (zoneData.ZoneRateGroup != null) ? zoneData.ZoneRateGroup.NormalRate : null;
            RateToChange newRate = zoneData.NormalRateToChange;

            if (currentRate != null && newRate != null)
                beginEffectiveDates.Add(Utilities.Min(currentRate.BED, newRate.BED));
            else if (currentRate != null)
                beginEffectiveDates.Add(currentRate.BED);
            else if (newRate != null)
                beginEffectiveDates.Add(newRate.BED);

            return beginEffectiveDates.Max();
        }

        #endregion
    }
}
