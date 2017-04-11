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
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            if (ratePlanContext.IntersectedSellingProductZoneRatesByZone == null)
                return true;

            var countryToAdd = context.Target as CustomerCountryToAdd;

            IEnumerable<ExistingZone> countryZones = ratePlanContext.ExistingZonesByCountry.GetRecord(countryToAdd.CountryId);

            if (countryZones == null || countryZones.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("SaleZones of Country '{0}' were not found", countryToAdd.CountryId));

            DateTime? validCountryBED = null;

            foreach (ExistingZone countryZone in countryZones)
            {
                IEnumerable<SaleRate> zoneRates = ratePlanContext.IntersectedSellingProductZoneRatesByZone.GetRecord(countryZone.ZoneId);

                if (zoneRates == null || zoneRates.Count() == 0)
                    continue;

                if (zoneRates.FindAllRecords(x => !x.EED.HasValue || x.EED.Value > countryToAdd.BED).Count() > 1)
                {
                    DateTime lastRateBED = zoneRates.Last().BED;
                    if (!validCountryBED.HasValue || validCountryBED.Value < lastRateBED)
                        validCountryBED = lastRateBED;
                }
            }

            if (validCountryBED.HasValue)
            {
                var countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(countryToAdd.CountryId);
                string validCountryBEDAsString = UtilitiesManager.GetDateTimeAsString(validCountryBED.Value);
                context.Message = string.Format("BED of Country '{0}' must be greater than or equal to '{1}'", countryName, validCountryBEDAsString);
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var countryToAdd = target as CustomerCountryToAdd;
            return string.Format("One or more SaleZones of Country '{0}' inherit multiple SaleRates from the assigned SellingProducts", countryToAdd.CountryId);
        }
    }
}
