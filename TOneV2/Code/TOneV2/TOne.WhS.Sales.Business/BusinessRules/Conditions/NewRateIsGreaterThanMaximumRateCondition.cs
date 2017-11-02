using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NewRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allZoneData = context.Target as AllDataByZone;

            if (allZoneData.DataByZoneList == null || allZoneData.DataByZoneList.Count() == 0)
                return true;

            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            var invalidCountryNames = new HashSet<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (DataByZone zoneData in allZoneData.DataByZoneList)
            {
                string countryName = countryManager.GetCountryName(zoneData.CountryId);

                if (invalidCountryNames.Contains(countryName))
                    continue;

                if (zoneData.NormalRateToChange != null)
                {
                    int rateToChangeCurrencyId = ratePlanContext.GetRateToChangeCurrencyId(zoneData.NormalRateToChange);
                    decimal convertedMaximumRate = ratePlanContext.GetMaximumRateConverted(rateToChangeCurrencyId);

                    if (zoneData.NormalRateToChange.NormalRate > convertedMaximumRate)
                    {
                        invalidCountryNames.Add(countryName);
                        continue;
                    }
                }

                if (zoneData.OtherRatesToChange != null && zoneData.OtherRatesToChange.Count > 0)
                {
                    foreach (RateToChange otherRateToChange in zoneData.OtherRatesToChange)
                    {
                        int rateToChangeCurrencyId = ratePlanContext.GetRateToChangeCurrencyId(zoneData.NormalRateToChange);
                        decimal convertedMaximumRate = ratePlanContext.GetMaximumRateConverted(rateToChangeCurrencyId);

                        if (otherRateToChange.NormalRate > convertedMaximumRate)
                        {
                            invalidCountryNames.Add(countryName);
                            break;
                        }
                    }
                }
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("The new rates of the following countries are greater than the maximum rate '{0}': {1}", ratePlanContext.MaximumRate, string.Join(", ", invalidCountryNames));
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
