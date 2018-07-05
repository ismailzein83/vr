using System;
using System.Linq;
using TOne.WhS.Deal.Business;
using Vanrise.Common.Business;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using System.Collections.Generic;
using Vanrise.BusinessProcess.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;


namespace TOne.WhS.Deal.BusinessProcessRules
{
    public class RPValidateStopSellingCountryCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return target is CustomerCountryToChange;
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();
            var customerCountryToChange = context.Target as CustomerCountryToChange;
            var dealDefinitionManager = new DealDefinitionManager();
            var salezoneManager = new SaleZoneManager();
            var zoneMessages = new List<String>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            foreach (var changedCountrie in customerCountryToChange.ChangedExistingCustomerCountries)
            {
                var countryId = changedCountrie.CustomerCountryEntity.CountryId;
                var countrySalezones = salezoneManager.GetSaleZonesByCountryId(ratePlanContext.OwnerSellingNumberPlanId, countryId, ratePlanContext.EffectiveDate);
                foreach (var saleZone in countrySalezones)
                {
                    var dealId = dealDefinitionManager.IsZoneIncludedInDeal(ratePlanContext.OwnerId, saleZone.SaleZoneId, ratePlanContext.EffectiveDate, true);
                    if (dealId.HasValue)
                    {
                        //  var deal = new DealDefinitionManager().GetDealDefinition(dealId.Value);
                        var countryName = new CountryManager().GetCountryName(countryId);
                        zoneMessages.Add(countryName);
                        break;
                    }
                }
            }
            if (zoneMessages.Any())
            {
                string zoneMessagesString = string.Join(",", zoneMessages);
                context.Message = String.Format("Following countries having zones included in effective deals and they cannot be ended: {0}", zoneMessagesString);
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
