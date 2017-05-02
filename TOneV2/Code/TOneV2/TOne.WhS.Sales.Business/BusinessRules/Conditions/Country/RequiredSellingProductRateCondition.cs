using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class RequiredSellingProductRateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
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

            var countryToAdd = context.Target as CustomerCountryToAdd;

            IEnumerable<ExistingZone> countryZones = ratePlanContext.ExistingZonesByCountry.GetRecord(countryToAdd.CountryId);
            IntersectedSellingProductZoneRatesByZone spZoneRatesByZone = ratePlanContext.IntersectedSellingProductZoneRatesByZone;

            if (countryZones == null || countryZones.Count() == 0)
                throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("Zones of Country '{0}' were not found"));

            string errorMessage = "The Zones of newly sold Countries must all have Rates at the level of the assigned Selling Products";

            if (spZoneRatesByZone == null || spZoneRatesByZone.Count == 0)
            {
                context.Message = errorMessage;
                return false;
            }

            foreach (ExistingZone countryZone in countryZones)
            {
                List<SaleRate> spZoneRates = spZoneRatesByZone.GetRecord(countryZone.ZoneId);

                if (spZoneRates == null || spZoneRates.Count == 0 || !spZoneRates.Any(spZoneRate => spZoneRate.IsEffective(countryToAdd.BED)))
                {
                    context.Message = errorMessage;
                    return false;
                }
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return "The Zones of newly sold Countries must all have Rates at the level of the assigned Selling Products";
        }
    }
}
