using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class CustomerZoneInheritsMultipleSellingProductZoneRatesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CustomerCountryToAdd;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            //IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            //if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
            //    return true;

            //if (ratePlanContext.IntersectedSellingProductZoneRatesByZone == null)
            //    return true;

            //var countryToAdd = context.Target as CustomerCountryToAdd;

            //IEnumerable<ExistingZone> countryZones = ratePlanContext.ExistingZonesByCountry.GetRecord(countryToAdd.CountryId);

            //if (countryZones == null || countryZones.Count() == 0)
            //    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZones of Country '{0}' were not found", countryToAdd.CountryId));

            //var invalidZoneNames = new List<string>();
            //var saleZoneManager = new SaleZoneManager();

            //foreach (ExistingZone countryZone in countryZones)
            //{
            //    IEnumerable<SaleRate> zoneRates = ratePlanContext.IntersectedSellingProductZoneRatesByZone.GetRecord(countryZone.ZoneId);

            //    if (zoneRates == null || zoneRates.Count() == 0)
            //        continue;

            //    if (zoneRates.FindAllRecords(x => x.IsEffective(countryToAdd.BED)).Count() > 1)
            //    {
            //        invalidZoneNames.Add(countryZone.Name);
            //    }
            //}

            //if (invalidZoneNames.Count > 0)
            //{
            //    context.Message = string.Format("The following Zones inherit multiple Rates from the Customer's SellingProducts", string.Join(",", invalidZoneNames));
            //    return false;
            //}

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var countryToAdd = target as CustomerCountryToAdd;
            return string.Format("One or more SaleZones of Country '{0}' inherit multiple SaleRates from the assigned SellingProducts", countryToAdd.CountryId);
        }
    }
}
