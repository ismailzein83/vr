using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Business;
using TOne.WhS.Sales.Entities;
using Vanrise.Common.Business;

namespace TOne.WhS.Sales.Business.BusinessRules
{
    public class NewCountryBEDIsLessThanRetroactiveDateCondition : Vanrise.BusinessProcess.Entities.BusinessRuleCondition
    {
        public override bool ShouldValidate(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            return target is CustomerCountryToAdd;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var countryToAdd = context.Target as CustomerCountryToAdd;
            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            if (countryToAdd.BED < ratePlanContext.RetroactiveDate)
            {
                string countryName = new CountryManager().GetCountryName(countryToAdd.CountryId);
                string beginEffectiveDate = UtilitiesManager.GetDateTimeAsString(countryToAdd.BED);
                context.Message = string.Format("BED '{0}' of Country '{1}' must be greater than or equal to the Retroactive date", beginEffectiveDate, countryName);
                return false;
            }

            return true;
        }

        public override string GetMessage(Vanrise.BusinessProcess.Entities.IRuleTarget target)
        {
            var countryToAdd = target as CustomerCountryToAdd;
            string countryName = new CountryManager().GetCountryName(countryToAdd.CountryId);
            string beginEffectiveDate = UtilitiesManager.GetDateTimeAsString(countryToAdd.BED);
            return string.Format("BED '{0}' of Country '{1}' must be greater than or equal to the Retroactive date", beginEffectiveDate, countryName);
        }
    }
}
