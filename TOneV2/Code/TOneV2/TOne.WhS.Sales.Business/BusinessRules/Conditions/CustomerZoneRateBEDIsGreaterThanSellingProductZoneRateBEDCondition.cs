using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CustomerZoneRateBEDIsGreaterThanSellingProductZoneRateBEDCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            return true;

            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer)
                return true;

            if (ratePlanContext.EffectiveAfterCustomerZoneRatesByZone == null || ratePlanContext.EffectiveAfterCustomerZoneRatesByZone.Count == 0)
                return true;

            var zoneData = context.Target as DataByZone;
            var errorMessages = new List<string>();

            if (zoneData.NormalRateToChange != null)
            {
                DateTime? maxNormalRateBED = GetMaxRateBED(ratePlanContext.EffectiveAfterCustomerZoneRatesByZone, zoneData.ZoneId, null);

                if (maxNormalRateBED.HasValue && maxNormalRateBED.Value > zoneData.NormalRateToChange.BED)
                {
                    errorMessages.Add(string.Format("BED '{0}' of rate of type 'Normal' is invalid and it must be greater than or equal to '{1}'", UtilitiesManager.GetDateTimeAsString(zoneData.NormalRateToChange.BED), UtilitiesManager.GetDateTimeAsString(maxNormalRateBED.Value)));
                }
            }

            if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
            {
                var rateTypeManager = new Vanrise.Common.Business.RateTypeManager();

                foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                {
                    DateTime? maxOtherRateBED = GetMaxRateBED(ratePlanContext.EffectiveAfterCustomerZoneRatesByZone, zoneData.ZoneId, otherRateToChange.RateTypeId);

                    if (maxOtherRateBED.HasValue && maxOtherRateBED > otherRateToChange.BED)
                    {
                        string rateTypeName = rateTypeManager.GetRateTypeName(otherRateToChange.RateTypeId.Value);
                        errorMessages.Add(string.Format("BED '{0}' of rate of type '{1}' is invalid and it must be greater than or equal to '{2}'", UtilitiesManager.GetDateTimeAsString(otherRateToChange.BED), rateTypeName, UtilitiesManager.GetDateTimeAsString(maxOtherRateBED.Value)));
                    }
                }
            }

            if (errorMessages.Count > 0)
            {
                context.Message = string.Format("The following rates of zone '{0}' have invalid BEDs: {1}", zoneData.ZoneName, string.Join(",", errorMessages));
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var zoneData = target as DataByZone;
            return string.Format("Some rates of zone '{0}' have invalid BEDs", zoneData.ZoneName);
        }

        private DateTime? GetMaxRateBED(EffectiveAfterCustomerZoneRatesByZone customerRatesByZone, long zoneId, int? rateTypeId)
        {
            Dictionary<RateTypeKey, List<SaleRate>> zoneRatesByType;

            if (customerRatesByZone.TryGetValue(zoneId, out zoneRatesByType))
            {
                List<SaleRate> zoneRatesOfType;
                var rateTypeKey = new RateTypeKey() { RateTypeId = rateTypeId };

                if (zoneRatesByType.TryGetValue(rateTypeKey, out zoneRatesOfType))
                {
                    return zoneRatesOfType.Max(x => x.BED);
                }
            }

            return null;
        }
    }
}
