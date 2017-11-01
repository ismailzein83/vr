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
            return target is AllCustomerCountriesToAdd;
        }

        public override bool Validate(Vanrise.BusinessProcess.Entities.IBusinessRuleConditionValidateContext context)
        {
            var allCountriesToAdd = context.Target as AllCustomerCountriesToAdd;

            if (allCountriesToAdd.CustomerCountriesToAdd == null || allCountriesToAdd.CustomerCountriesToAdd.Count() == 0)
                return true;

            IRatePlanContext ratePlanContext = context.GetExtension<IRatePlanContext>();

            var invalidCountryNames = new List<string>();
            var countryManager = new Vanrise.Common.Business.CountryManager();

            foreach (CustomerCountryToAdd countryToAdd in allCountriesToAdd.CustomerCountriesToAdd)
            {
                if (countryToAdd.BED < ratePlanContext.RetroactiveDate)
                    invalidCountryNames.Add(countryManager.GetCountryName(countryToAdd.CountryId));
            }

            if (invalidCountryNames.Count > 0)
            {
                string retroactiveDateString = ratePlanContext.RetroactiveDate.ToString(ratePlanContext.DateFormat);
                context.Message = string.Format("BEDs of the following countries must be greater than or equal to the retroactive date '{0}': {1}", retroactiveDateString, string.Join(", ", invalidCountryNames));
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
