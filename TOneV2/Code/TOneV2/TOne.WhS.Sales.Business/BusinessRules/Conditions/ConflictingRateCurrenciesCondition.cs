using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class ConflictingRateCurrenciesCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            var countryData = context.Target as CountryData;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct || countryData.IsCountryNew)
                return true;

            int sellingProductId = new CarrierAccountManager().GetSellingProductId(ratePlanContext.OwnerId);

            string errorMessage;
            bool areCountryZoneRateCurrenciesValid = BusinessRuleUtilities.ValidateCountryZoneRateCurrencies(countryData, ratePlanContext, sellingProductId, out errorMessage);

            context.Message = errorMessage;
            return areCountryZoneRateCurrenciesValid;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
