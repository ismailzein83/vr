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
            return target is AllCustomerCountriesToAdd;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            var countriesToAdd = context.Target as AllCustomerCountriesToAdd;

            if (countriesToAdd.CustomerCountriesToAdd == null || countriesToAdd.CustomerCountriesToAdd.Count() == 0)
                return true;

            var invalidCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (CustomerCountryToAdd countryToAdd in countriesToAdd.CustomerCountriesToAdd)
            {
                IEnumerable<ExistingZone> countryZones = ratePlanContext.EffectiveAndFutureExistingZonesByCountry.GetRecord(countryToAdd.CountryId);
                string countryName = countryManager.GetCountryName(countryToAdd.CountryId);

                if (countryZones == null || countryZones.Count() == 0)
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The existing zones of country '{0}' were not found", countryName));

                foreach (ExistingZone countryZone in countryZones)
                {
                    DateTime effectiveOn = Utilities.Max(countryToAdd.BED, countryZone.BED);

                    if (countryZone.EED.HasValue && countryZone.EED.Value <= effectiveOn)
                        continue;

                    ZoneInheritedRates zoneRates = ratePlanContext.InheritedRatesByZoneId.GetRecord(countryZone.ZoneId);

                    if (zoneRates == null || zoneRates.NormalRates == null || zoneRates.NormalRates.Count == 0 || !zoneRates.NormalRates.Any(x => x.IsEffective(effectiveOn)))
                    {
                        invalidCountryNames.Add(countryName);
                        break;
                    }
                }
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Selling countries must have inherited rates at sell dates. Violated countries: {0}", string.Join(", ", invalidCountryNames));
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
