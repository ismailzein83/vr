using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;


namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NewRateIsGreaterThanMaximumRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        CurrencyManager currencyManager = new CurrencyManager();
        int rateToChangeCurrencyId;
        decimal convertedMaximumRate;
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

                if ((zoneData.IsCustomerCountryNew.HasValue && zoneData.IsCustomerCountryNew.Value) || invalidCountryNames.Contains(countryName))
                    continue;

                if (BusinessRuleUtilities.IsAnyZoneRateGreaterThanMaxRate(zoneData, ratePlanContext))
                {
                     rateToChangeCurrencyId = ratePlanContext.GetRateToChangeCurrencyId(zoneData.NormalRateToChange);
                     convertedMaximumRate = ratePlanContext.GetMaximumRateConverted(rateToChangeCurrencyId);
                    invalidCountryNames.Add(countryName);
                }
                   
            }

            if (invalidCountryNames.Count > 0)
            {
                currencyManager.GetCurrencySymbol(rateToChangeCurrencyId);
                context.Message = string.Format("New rates of following sold country(ies) are greater than maximum rate '{0} {1}': {2}", convertedMaximumRate, currencyManager.GetCurrencySymbol(rateToChangeCurrencyId), string.Join(", ", invalidCountryNames));
                return false;
            }

            return true;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        #endregion
    }
}
