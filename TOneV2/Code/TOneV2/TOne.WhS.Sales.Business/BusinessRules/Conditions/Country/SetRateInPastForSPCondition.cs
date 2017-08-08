using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules.Conditions.Country
{
    public class SetRateInPastForSPCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer || ratePlanContext.IsFirstSellingProductOffer.Value)
                return true;

            var countryData = context.Target as CountryData;

            if (countryData.ZoneDataByZoneId != null)
            {
                string countryName = new CountryManager().GetCountryName(countryData.CountryId);
                string errorMessage = string.Format("Some of the zones of country '{0}' have rates with a BED that's less than today", countryName);

                foreach (var zoneElement in countryData.ZoneDataByZoneId)
                {
                    if (zoneElement.Value.NormalRateToChange != null && zoneElement.Value.NormalRateToChange.BED < DateTime.Today)
                    {
                        context.Message = errorMessage;
                        return false;
                    }

                    if (zoneElement.Value.OtherRatesToChange != null)
                    {
                        foreach (var rate in zoneElement.Value.OtherRatesToChange)
                        {
                            if (rate.BED < DateTime.Today)
                            {
                                context.Message = errorMessage;
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
