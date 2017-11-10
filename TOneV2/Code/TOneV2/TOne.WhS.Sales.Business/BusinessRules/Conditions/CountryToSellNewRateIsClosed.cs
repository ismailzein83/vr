using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CountryToSellNewRateIsClosed : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var countryData = context.Target as CountryData;

            if (countryData.ZoneDataByZoneId == null || countryData.ZoneDataByZoneId.Count == 0)
                return true;

            var invalidZoneNames = new List<string>();

            foreach (DataByZone zoneData in countryData.ZoneDataByZoneId.Values)
            {
                if (!zoneData.IsCustomerCountryNew.HasValue || !zoneData.IsCustomerCountryNew.Value)
                    continue;

                if (BusinessRuleUtilities.IsAnyZoneNewRateEnded(zoneData))
                    invalidZoneNames.Add(zoneData.ZoneName);
            }

            if (invalidZoneNames.Count > 0)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryData.CountryId);
                context.Message = string.Format("Can not define a new rate with EED for selling country '{0}'. Violated zone(s): {1}", countryName, string.Join(",", invalidZoneNames));
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
