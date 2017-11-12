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
    public class NewCountryHasAPendingClosedZoneCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
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

            var allCountriesToAdd = context.Target as AllCustomerCountriesToAdd;

            if (allCountriesToAdd.CustomerCountriesToAdd == null || allCountriesToAdd.CustomerCountriesToAdd.Count() == 0)
                return true;

            var invalidCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (CustomerCountryToAdd countryToAdd in allCountriesToAdd.CustomerCountriesToAdd)
            {
                List<ExistingZone> countryZones;

                if (!ratePlanContext.EffectiveAndFutureExistingZonesByCountry.TryGetValue(countryToAdd.CountryId, out countryZones))
                    throw new Vanrise.Entities.VRBusinessException(string.Format("No existing zones were found for country '{0}'", countryToAdd.CountryId));

                if (countryZones.FindAllRecords(x => x.IsEffectiveOrFuture(countryToAdd.BED)).Any(x => x.EED.HasValue))
                    invalidCountryNames.Add(countryManager.GetCountryName(countryToAdd.CountryId));
            }

            if (invalidCountryNames.Count > 0)
            {
                context.Message = string.Format("Can not sell country(ies) having pending closed zones. Violated country(ies): {0}", string.Join(", ", invalidCountryNames));
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
