using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class SellingProductZoneRateIsEndedCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is DataByZone;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.Customer || (ratePlanContext.IsFirstSellingProductOffer.HasValue && ratePlanContext.IsFirstSellingProductOffer.Value))
                return true;

            var zoneData = context.Target as DataByZone;

            if (zoneData.NormalRateToClose != null)
            {
                context.Message = string.Format("Can not close rates of a selling product. Violated zone(s): {0}", zoneData.ZoneName);
                return false;
            }

            if (zoneData.NormalRateToChange != null && zoneData.NormalRateToChange.EED.HasValue)
            {
                string countryName = new Vanrise.Common.Business.CountryManager().GetCountryName(zoneData.CountryId);
                context.Message = string.Format("Can not define a new rate with EED for country '{0}'. Violated zone(s): {1}", countryName, zoneData.ZoneName);
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
