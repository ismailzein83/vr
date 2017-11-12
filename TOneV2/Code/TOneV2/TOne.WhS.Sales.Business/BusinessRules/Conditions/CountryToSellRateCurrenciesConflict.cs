using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CountryToSellRateCurrenciesConflict : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CountryData;
        }
        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var ratePlanContext = context.GetExtension<IRatePlanContext>();
            var countryData = context.Target as CountryData;

            if (ratePlanContext.OwnerType == SalePriceListOwnerType.SellingProduct || !countryData.IsCountryNew)
                return true;

            string errorMessage;
            bool doZoneRateCurrenciesConflict = BusinessRuleUtilities.DoZoneRateCurrenciesConflict(countryData, ratePlanContext, out errorMessage);

            context.Message = errorMessage;
            return !doZoneRateCurrenciesConflict;
        }
        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            throw new NotImplementedException();
        }
    }
}
