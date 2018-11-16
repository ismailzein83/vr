using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class CountryToSellRateEqualToZeroCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is AllDataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            ratePlanContext.ThrowIfNull("ratePlanContext");

            if (ratePlanContext.AllowRateZero)
                return true;

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

                if (BusinessRuleUtilities.IsAnyZoneRateEqualToZero(zoneData))
                    invalidCountryNames.Add(countryName);
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Following selling country(ies) have rates equal zero: {0}", string.Join(", ", invalidCountryNames));
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
