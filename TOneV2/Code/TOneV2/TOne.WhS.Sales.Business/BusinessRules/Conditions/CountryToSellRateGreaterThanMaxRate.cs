using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CountryToSellRateGreaterThanMaxRate : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var allDataByZone = context.Target as AllDataByZone;

            if (allDataByZone.DataByZoneList == null || allDataByZone.DataByZoneList.Count() == 0)
                return true;

            var invalidCountryNames = new HashSet<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (DataByZone zoneData in allDataByZone.DataByZoneList)
            {
                string countryName = countryManager.GetCountryName(zoneData.CountryId);

                if (!zoneData.IsCustomerCountryNew.HasValue || !zoneData.IsCustomerCountryNew.Value || invalidCountryNames.Contains(countryName))
                    continue;

                if (BusinessRuleUtilities.IsAnyZoneRateGreaterThanMaxRate(ratePlanContext.OwnerId, ratePlanContext.OwnerType, zoneData, ratePlanContext))
                    invalidCountryNames.Add(countryName);
            }

            if (invalidCountryNames.Count > 0)
            {
                Vanrise.Common.Business.CurrencyManager currencyManager = new Vanrise.Common.Business.CurrencyManager ();
                Vanrise.Common.Business.CurrencyExchangeRateManager currencyExchangeRateManager = new Vanrise.Common.Business.CurrencyExchangeRateManager();
                Vanrise.Common.Business.GeneralSettingsManager generalSettingsManager = new Vanrise.Common.Business.GeneralSettingsManager();

                var pricelistCurrencySymbol=currencyManager.GetCurrencySymbol(ratePlanContext.CurrencyId);

                var systemCurrency = currencyManager.GetSystemCurrency();
                var convertedMaximumRate = currencyExchangeRateManager.ConvertValueToCurrency(ratePlanContext.MaximumRate, systemCurrency.CurrencyId, ratePlanContext.CurrencyId, DateTime.Now);
                var roundedMaximumRate = Math.Round(convertedMaximumRate, generalSettingsManager.GetLongPrecisionValue());

                context.Message = string.Format("New rates of following selling country(ies) are greater than maximum rate '{0} {1}': {2}", roundedMaximumRate, pricelistCurrencySymbol, string.Join(", ", invalidCountryNames));
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
