﻿using System;
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
            var countryMessages = new List<String>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var countryId = customerCountryToChange.CountryId;
            var countrySaleZones = salezoneManager.GetSaleZonesByCountryId(ratePlanContext.OwnerSellingNumberPlanId, countryId, ratePlanContext.EffectiveDate);
            foreach (var saleZone in countrySaleZones)
            {
                var dealId = dealDefinitionManager.IsZoneIncludedInEffectiveDeal(ratePlanContext.OwnerId, saleZone.SaleZoneId, DateTime.Now, true);
                if (dealId.HasValue)
                {
                    var countryName = new CountryManager().GetCountryName(countryId);
                    countryMessages.Add(countryName);
                    break;
                }
            }

            if (countryMessages.Any())
            {
                string countryMessagesString = string.Join(",", countryMessages);
                context.Message = $"Cannot Close Country(ies) having zones included in effective deals. Following countries are:  {countryMessagesString}";
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
