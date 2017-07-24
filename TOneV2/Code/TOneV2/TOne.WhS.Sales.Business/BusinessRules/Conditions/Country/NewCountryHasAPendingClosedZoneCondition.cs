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
            return target is CustomerCountryToAdd;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct)
                return true;

            CustomerCountryToAdd newCountry = context.Target as CustomerCountryToAdd;
            List<ExistingZone> countryZones;

            if (!ratePlanContext.EffectiveAndFutureExistingZonesByCountry.TryGetValue(newCountry.CountryId, out countryZones))
                throw new Vanrise.Entities.VRBusinessException(string.Format("No existing zones were found for country '{0}'", newCountry.CountryId));

            if (countryZones.FindAllRecords(x => x.IsEffectiveOrFuture(newCountry.BED)).Any(x => x.EED.HasValue))
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(newCountry.CountryId);
                context.Message = string.Format("Country '{0}' cannot be sold to the customer because it has at least one pending closed zone", countryName);
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
