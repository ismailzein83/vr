using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class DraftNewRateIsClosed : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.IsFirstSellingProductOffer.HasValue && ratePlanContext.IsFirstSellingProductOffer.Value)
                return true;

            var countryData = context.Target as CountryData;

            if (countryData.ZoneDataByZoneId == null || countryData.ZoneDataByZoneId.Count == 0)
                return true;

            var invalidZoneNames = new List<string>();

            foreach (DataByZone zoneData in countryData.ZoneDataByZoneId.Values)
            {
                if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
                {
                    invalidZoneNames.Add(zoneData.ZoneName);
                    break;
                }

                if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
                {
                    foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                    {
                        if (otherRateToChange.EED.HasValue)
                        {
                            invalidZoneNames.Add(zoneData.ZoneName);
                            break;
                        }
                    }
                }
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);
                context.Message = string.Format("Cannot end the new rates of the following zones of country '{0}': {1}", countryName, string.Join(",", invalidZoneNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
