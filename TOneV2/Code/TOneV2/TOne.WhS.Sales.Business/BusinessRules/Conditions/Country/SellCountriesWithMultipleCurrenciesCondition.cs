using System;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SellCountriesWithMultipleCurrenciesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is ZoneDataByCountryIds;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            return true;
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            var zoneDataByCountryIds = context.Target as ZoneDataByCountryIds;
            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;
            var carrierAccountManager = new CarrierAccountManager();
            var customerId = ratePlanContext.OwnerId;
            var customerCurrencyId = carrierAccountManager.GetCarrierAccountCurrencyId(customerId);
            var sellingProductId = carrierAccountManager.GetSellingProductId(customerId);
            var sellingProductCurrencyId = new SellingProductManager().GetSellingProductCurrencyId(sellingProductId);

            var invalidCustomerCurrencyCountryName = new List<string>();
            var invalidSellingProductCurrencyCountryName = new List<string>();

            foreach (KeyValuePair<int, List<DataByZone>> kvp in zoneDataByCountryIds)
            {
                string countryName = new CountryManager().GetCountryName(kvp.Key);
                foreach (DataByZone zoneData in kvp.Value)
                {
                    if (zoneData.NormalRateToChange != null)
                    {
                        var normalRate = zoneData.NormalRateToChange;
                        if (!normalRate.CurrencyId.HasValue)
                            continue;

                        if (normalRate.CurrencyId.Value != customerCurrencyId)
                        {
                            invalidCustomerCurrencyCountryName.Add(countryName);
                        }
                        else if (normalRate.CurrencyId.Value != sellingProductCurrencyId)
                        {
                            invalidSellingProductCurrencyCountryName.Add(countryName);
                        }
                    }

                }
            }
            if (invalidCustomerCurrencyCountryName.Count > 0)
            {
                context.Message = $"Selling countries for a customer cannot be done with currency different than customer currency : {string.Join(", ", invalidCustomerCurrencyCountryName)}";
                return false;
            }
            if (invalidSellingProductCurrencyCountryName.Count > 0)
            {
                context.Message = $"Selling countries for a customer cannot be done with currency different than selling product currency : {string.Join(", ", invalidSellingProductCurrencyCountryName)}";
                return false;
            }
            return true;
        }

        public override string GetMessage(IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
